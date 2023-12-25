using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Services;
using Application.Parameters;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Entities.Booking;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Statistics.Queries.GetStatisticByTimeStep

{
    public class GetStatisticByTimeStepQuery : RequestParameter, IRequest<PaginatedResult<GetStatisticByTimeStepResponse>>
    {
        [Required]
        public StatisticTimeStep TimeStep { get; set; }
        public long CinemaId { get; set; }
    }

    public class GetStatisticByTimeStepQueryHandler : IRequestHandler<GetStatisticByTimeStepQuery, PaginatedResult<GetStatisticByTimeStepResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly ITimeZoneService _timeZoneService;

        public GetStatisticByTimeStepQueryHandler(
            IBookingRepository bookingRepository,
            IEnumService enumService,
            ITimeZoneService timeZoneService)
        {
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _timeZoneService = timeZoneService;
        }
        public async Task<PaginatedResult<GetStatisticByTimeStepResponse>> Handle(GetStatisticByTimeStepQuery request, CancellationToken cancellationToken)
        {
            List<GetStatisticByTimeStepResponse> statistics = new List<GetStatisticByTimeStepResponse>();

            if (request.TimeStep == StatisticTimeStep.Day)
                statistics = HandleDayStep();
            else if (request.TimeStep == StatisticTimeStep.Week)
                statistics = HandleWeekStep();
            else
                statistics = HandleMonthStep();

            var totalCount = statistics.Count;

            List<GetStatisticByTimeStepResponse> result;
            if (!request.IsExport)
                result = statistics.Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize).ToList();
            else
                result = statistics;

            return PaginatedResult<GetStatisticByTimeStepResponse>.Success(result, totalCount, request.PageNumber, request.PageSize);
        }

        public List<GetStatisticByTimeStepResponse> HandleDayStep()
        {
            DateTime currentDate = _timeZoneService.GetGMT7Time().Date;
            List<GetStatisticByTimeStepResponse> statistics = new List<GetStatisticByTimeStepResponse>();

            var bookingGroups = from booking in _bookingRepository.Entities.AsEnumerable()
                                where !booking.IsDeleted
                                && booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                                && booking.BookingDate.HasValue
                                group booking by booking.BookingDate.Value.Date into bookingGroup
                                orderby bookingGroup.Key descending
                                select new
                                {
                                    Date = bookingGroup.Key.Date,
                                    Revenue = bookingGroup.Sum(booking => booking.RequiredAmount.HasValue ? booking.RequiredAmount.Value : 0),
                                    BookingIds = bookingGroup.Select(booking => booking.Id).ToList()
                                };

            if (bookingGroups.Count() == 0)
                return statistics;

            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = bookingGroups.Last().Date; date <= currentDate; date = date.AddDays(1))
            {
                allDates.Add(date);
            }

            statistics = (from date in allDates.OrderDescending().AsEnumerable()
                          join bookingGroup in bookingGroups
                          on date equals bookingGroup.Date
                          into dateBookingGroups
                          from dateBookingGroup in dateBookingGroups.DefaultIfEmpty()
                          select new GetStatisticByTimeStepResponse
                          {
                              Label = $"{date:dd/MM/yyyy}",
                              Revenue = dateBookingGroup != null ? dateBookingGroup.Revenue : 0,
                              NumberOfBookings = dateBookingGroup != null ? dateBookingGroup.BookingIds.Count() : 0,
                              NumberOfTickets = dateBookingGroup != null ? dateBookingGroup.BookingIds.Sum(id => _bookingRepository.GetBookingNumberOfTickets(id)) : 0
                          }).ToList();
            return statistics;
        }
        public List<GetStatisticByTimeStepResponse> HandleWeekStep()
        {
            DateTime currentDate = _timeZoneService.GetGMT7Time();
            List<GetStatisticByTimeStepResponse> statistics = new List<GetStatisticByTimeStepResponse>();

            var bookingGroups = from booking in _bookingRepository.Entities.AsEnumerable()
                                where !booking.IsDeleted
                                && booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                                && booking.BookingDate.HasValue
                                let weekStart = booking.BookingDate.Value.Date
                                .AddDays(booking.BookingDate.Value.DayOfWeek == DayOfWeek.Sunday ? -6 : -(int)booking.BookingDate.Value.DayOfWeek + 1)
                                group booking by new { WeekStart = weekStart }
                                into bookingGroup
                                orderby bookingGroup.Key.WeekStart descending
                                select new
                                {
                                    WeekStart = bookingGroup.Key.WeekStart,
                                    Revenue = bookingGroup.Sum(booking => booking.RequiredAmount.HasValue ? booking.RequiredAmount.Value : 0),
                                    BookingIds = bookingGroup.Select(booking => booking.Id).ToList()
                                };

            if (bookingGroups.Count() == 0)
                return statistics;

            DateTime startWeek = bookingGroups.Last().WeekStart;
            DateTime currentWeek = currentDate.AddDays(currentDate.DayOfWeek == DayOfWeek.Sunday ? -6 : -(int)currentDate.DayOfWeek + 1).Date;
            List<DateTime> allWeeks = new List<DateTime>();
            for (DateTime week = startWeek; week <= currentWeek; week = week.AddDays(7))
            {
                allWeeks.Add(week);
            }

            statistics = (from week in allWeeks.OrderDescending().AsEnumerable()
                          join bookingGroup in bookingGroups
                          on week.Date equals bookingGroup.WeekStart
                          into weekBookingGroups
                          from weekBookingGroup in weekBookingGroups.DefaultIfEmpty()
                          select new GetStatisticByTimeStepResponse
                          {
                              Label = $"{week.Date:dd/MM/yyyy}-{week.Date.AddDays(6):dd/MM/yyyy}",
                              Revenue = weekBookingGroup != null ? weekBookingGroup.Revenue : 0,
                              NumberOfBookings = weekBookingGroup != null ? weekBookingGroup.BookingIds.Count() : 0,
                              NumberOfTickets = weekBookingGroup != null ? weekBookingGroup.BookingIds.Sum(id => _bookingRepository.GetBookingNumberOfTickets(id)) : 0
                          }).ToList();
            return statistics;
        }
        public List<GetStatisticByTimeStepResponse> HandleMonthStep()
        {
            DateTime currentDate = _timeZoneService.GetGMT7Time();
            List<GetStatisticByTimeStepResponse> statistics = new List<GetStatisticByTimeStepResponse>();

            var bookingGroups = from booking in _bookingRepository.Entities.AsEnumerable()
                           where !booking.IsDeleted
                           && booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                           && booking.BookingDate.HasValue
                           group booking by new
                           {
                               month = booking.BookingDate.Value.Month,
                               year = booking.BookingDate.Value.Year,
                           } into bookingGroup
                           orderby bookingGroup.Key.year descending, bookingGroup.Key.month descending
                           select new
                           {
                               MonthYear = new DateTime(bookingGroup.Key.year, bookingGroup.Key.month, 1),
                               Revenue = bookingGroup.Sum(booking => booking.RequiredAmount.HasValue ? booking.RequiredAmount.Value : 0),
                               BookingIds = bookingGroup.Select(booking => booking.Id).ToList()
                           };

            if (bookingGroups.Count() == 0)
                return statistics;

            DateTime startMonth = bookingGroups.Last().MonthYear;
            List<DateTime> allMonths = new List<DateTime>();
            for (DateTime month = startMonth; month <= currentDate; month =  month.AddMonths(1))
            {
                allMonths.Add(month);
            }
            Console.WriteLine(string.Join(", ", allMonths));
            
            statistics = (from month in allMonths.OrderDescending().AsEnumerable()
                          join bookingGroup in bookingGroups
                          on month.Date equals bookingGroup.MonthYear.Date
                          into monthBookingGroups
                          from monthBookingGroup in monthBookingGroups.DefaultIfEmpty()
                          select new GetStatisticByTimeStepResponse
                          {
                              Label = $"{month:MM/yyyy}",
                              Revenue = monthBookingGroup != null? monthBookingGroup.Revenue : 0,
                              NumberOfBookings = monthBookingGroup != null ? monthBookingGroup.BookingIds.Count() : 0,
                              NumberOfTickets = monthBookingGroup != null ? monthBookingGroup.BookingIds.Sum(id => _bookingRepository.GetBookingNumberOfTickets(id)) : 0
                          }).ToList();
            return statistics;
        }
    }
}
