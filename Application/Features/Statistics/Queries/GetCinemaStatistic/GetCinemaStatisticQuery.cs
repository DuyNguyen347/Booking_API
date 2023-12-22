using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Application.Parameters;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Statistics.Queries.GetCinemaStatistic
{
    public class GetCinemaStatisticQuery: RequestParameter, IRequest<PaginatedResult<GetCinemaStatisticResponse>>
    {
        public StatisticsTimeOption TimeOption { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set;}
        public long CinemaId {  get; set; }
    }
    public class GetCinemaStatisticQueryHandler : IRequestHandler<GetCinemaStatisticQuery, PaginatedResult<GetCinemaStatisticResponse>>
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IEnumService _enumService;

        public GetCinemaStatisticQueryHandler(
            ICinemaRepository cinemaRepository,
            IRoomRepository roomRepository,
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ITimeZoneService timeZoneService,
            IEnumService enumService)
        {
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _timeZoneService = timeZoneService;
            _enumService = enumService;
        }

        public async Task<PaginatedResult<GetCinemaStatisticResponse>> Handle(GetCinemaStatisticQuery request, CancellationToken cancellationToken)
        {
            var cinemaStatistics = new List<GetCinemaStatisticResponse>();
            DateTime currentDate = _timeZoneService.GetGMT7Time();
            IEnumerable<Domain.Entities.Booking.Booking> bookings;

            if (request.TimeOption == StatisticsTimeOption.Today)
            {
                bookings = _bookingRepository.Entities
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date == currentDate.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                    .AsQueryable();
            }
            else if (request.TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentDate.AddDays((int)currentDate.DayOfWeek == 0 ? -7 : -(int)currentDate.DayOfWeek);
                bookings = _bookingRepository.Entities
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date > firstDateOfThisWeek.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                    .AsQueryable();
            }
            else if (request.TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDayOfThisMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                bookings = _bookingRepository.Entities
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date >= firstDayOfThisMonth.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                    .AsQueryable();
            }
            else if (request.TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDayOfThisYear = new DateTime(currentDate.Year, 1, 1);
                bookings = _bookingRepository.Entities
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date >= firstDayOfThisYear.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                    .AsQueryable();
            }
            else
            {
                DateTime ToTime = request.ToTime.HasValue ? request.ToTime.Value : currentDate;
                bookings = _bookingRepository.Entities
                .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                && (request.FromTime.HasValue && x.BookingDate.Value.Date >= request.FromTime.Value.Date && x.BookingDate.Value.Date <= ToTime.Date)
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                    .AsQueryable();
            }

            //Thuc hien loc ket qua
            var query = (from cinema in _cinemaRepository.Entities
                         where !cinema.IsDeleted && (request.CinemaId == 0 || cinema.Id == request.CinemaId)
                         join room in _roomRepository.Entities
                         on new { CinemaId = cinema.Id, IsDeleted = false } equals new { room.CinemaId, room.IsDeleted }
                         into cinemaRooms
                         from cinemaRoom in cinemaRooms.DefaultIfEmpty()
                         join schedule in _scheduleRepository.Entities
                         on new { RoomId = cinemaRoom.Id, IsDeleted = false } equals new { schedule.RoomId, schedule.IsDeleted }
                         into cinemaSchedules
                         from cinemaSchedule in cinemaSchedules.DefaultIfEmpty()
                         join booking in bookings
                         on cinemaSchedule.Id equals booking.ScheduleId
                         into cinemaBookings
                         from cinemaBooking in cinemaBookings.DefaultIfEmpty()
                         select new
                         {
                             cinemaInfo = cinema,
                             bookingInfo = cinemaBooking
                         });
            var queryGroupByCinema = query.GroupBy(x => x.cinemaInfo.Id);

            foreach (var cinemaBooking in queryGroupByCinema)
            {
                var cinemaStatistic = new GetCinemaStatisticResponse
                {
                    Id = cinemaBooking.First().cinemaInfo.Id,
                    Name = cinemaBooking.First().cinemaInfo.Name,
                    City = cinemaBooking.First().cinemaInfo.City,
                    Hotline = cinemaBooking.First().cinemaInfo.Hotline,
                    Longitude = cinemaBooking.First().cinemaInfo.Longitude,
                    Latitude = cinemaBooking.First().cinemaInfo.Latitude,
                    Address = cinemaBooking.First().cinemaInfo.Address
                };

                decimal revenue = 0;
                int numberOfTickets = 0;
                foreach (var booking in cinemaBooking)
                {
                    if (booking.bookingInfo != null)
                    {
                        revenue += booking.bookingInfo.RequiredAmount.HasValue ? booking.bookingInfo.RequiredAmount.Value : 0;
                        numberOfTickets += _bookingRepository.GetBookingNumberOfTickets(booking.bookingInfo.Id);
                    }
                }
                cinemaStatistic.Revenue = revenue;
                cinemaStatistic.NumberOfTickets = numberOfTickets;
                cinemaStatistics.Add(cinemaStatistic);
            }
            //tim kiem va sap xep
            IEnumerable<GetCinemaStatisticResponse> enumStatistics = cinemaStatistics;

            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var data = enumStatistics.Where(x => string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.Name, request.Keyword)
                                || StringHelper.Contains(x.Address, request.Keyword)
                                || StringHelper.Contains(x.City, request.Keyword)
                                || StringHelper.Contains(x.Hotline, request.Keyword)).AsQueryable();

            var totalCount = data.Count();

            if (!request.OrderBy.Contains("CreatedOn"))
                data = data.OrderBy(request.OrderBy);
            else
                data = data.OrderBy("Revenue desc");

            //Phan trang
            List<GetCinemaStatisticResponse> result;
            if (!request.IsExport)
                result = data.Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize).ToList();
            else
                result = data.ToList();

            return PaginatedResult<GetCinemaStatisticResponse>.Success(result, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
