using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;


namespace Application.Features.Statistics.Queries.GetDaytimeRangesStatistic
{
    public class GetDaytimeRangesStatisticQuery: IRequest<Result<List<GetDaytimeRangesStatisticResponse>>>
    {
        [Required]
        public StatisticsTimeOption TimeOption { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public string? OrderBy {  get; set; }
    }
    public class GetDaytimeRangesStatisticQueryHandler : IRequestHandler<GetDaytimeRangesStatisticQuery, Result<List<GetDaytimeRangesStatisticResponse>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITimeZoneService _timeZoneService;
        
        public GetDaytimeRangesStatisticQueryHandler(
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ITimeZoneService timeZoneService)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _timeZoneService = timeZoneService;
        }

        public async Task<Result<List<GetDaytimeRangesStatisticResponse>>> Handle(GetDaytimeRangesStatisticQuery request, CancellationToken cancellationToken)
        {
            var bookings = _bookingRepository
                .GetBookingsByTimeChoice(request.TimeOption, request.FromTime, request.ToTime)
                .AsQueryable();

            var timeRangesStatistic = new List<GetDaytimeRangesStatisticResponse>
            {
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour =  8, 
                    EndHour =  12,
                    FcsBookingRevenue = 0m,
                    FcsScheduleRevenue = 0m
                },
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour =  12,
                    EndHour = 16,
                    FcsBookingRevenue = 0m,
                    FcsScheduleRevenue = 0m
                },
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour = 16,
                    EndHour = 20,
                    FcsBookingRevenue = 0m,
                    FcsScheduleRevenue = 0m
                },
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour = 20,
                    EndHour = 24,
                    FcsBookingRevenue = 0m,
                    FcsScheduleRevenue = 0m
                }
            };

            var bookingQuery = (from booking in bookings
                         join schedule in _scheduleRepository.Entities
                         on new { Id = booking.ScheduleId, IsDeleted = false } equals new { schedule.Id, schedule.IsDeleted }
                         select new
                         {
                             bookingInfo = booking,
                             scheduleInfo = schedule
                         }).ToList();

            foreach (var bookingSchedule in bookingQuery)
            {
                var scheduleHour = bookingSchedule.scheduleInfo.StartTime.Hour;
                var timeRange = timeRangesStatistic.FirstOrDefault(range =>
                    range.StartHour <= scheduleHour
                    && scheduleHour < range.EndHour
                );
                if (timeRange != null)
                {
                    timeRange.FcsBookingRevenue += bookingSchedule.bookingInfo.RequiredAmount.HasValue? bookingSchedule.bookingInfo.RequiredAmount.Value:0;
                    timeRange.FcsBookingNumberOfBookings += 1;
                    timeRange.FcsBookingNumberOfTickets += _bookingRepository.GetBookingNumberOfTickets(bookingSchedule.bookingInfo.Id);
                    timeRange.FcsBookingNumberOfSchedules += 1;
                }
            }

            var schedules = GetScheduleByTimeOption(request).AsQueryable();

            var scheduleQuery = from schedule in schedules
                                join booking in _bookingRepository.Entities
                                on new { ScheduleId = schedule.Id, IsDeleted = false } equals new { booking.ScheduleId, booking.IsDeleted }
                                into scheduleBookings
                                from scheduleBooking in scheduleBookings.DefaultIfEmpty()
                                select new
                                {
                                    scheduleInfo = schedule,
                                    bookingInfo = scheduleBooking
                                };

            foreach (var scheduleBooking in scheduleQuery)
            {
                var scheduleHour = scheduleBooking.scheduleInfo.StartTime.Hour;
                var timeRange = timeRangesStatistic.FirstOrDefault(range =>
                    range.StartHour <= scheduleHour
                    && scheduleHour < range.EndHour
                );
                if ( timeRange != null)
                {
                    if (scheduleBooking.bookingInfo != null)
                    {
                        timeRange.FcsScheduleRevenue += scheduleBooking.bookingInfo.RequiredAmount.HasValue ? scheduleBooking.bookingInfo.RequiredAmount.Value : 0;
                        timeRange.FcsScheduleNumberOfBookings += 1;
                        timeRange.FcsScheduleNumberOfTickets += _bookingRepository.GetBookingNumberOfTickets(scheduleBooking.bookingInfo.Id);
                    }
                    timeRange.FcsScheduleNumberOfSchedules += 1;
                }
            }

            List<GetDaytimeRangesStatisticResponse> result;
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                IEnumerable<GetDaytimeRangesStatisticResponse> orderable = timeRangesStatistic;
                result = orderable.AsQueryable().OrderBy(request.OrderBy).ToList();
            }
            else
                result = timeRangesStatistic;

            return await Result<List<GetDaytimeRangesStatisticResponse>>.SuccessAsync(result);
        }
        public IEnumerable<Domain.Entities.Schedule.Schedule> GetScheduleByTimeOption(GetDaytimeRangesStatisticQuery request)
        {
            IEnumerable<Domain.Entities.Schedule.Schedule> schedules;

            DateTime currentDate = _timeZoneService.GetGMT7Time();

            if (request.TimeOption == StatisticsTimeOption.Today)
            {
                schedules = _scheduleRepository.Entities
                .Where(x => !x.IsDeleted && x.StartTime.Date == currentDate.Date);
            }
            else if (request.TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentDate.AddDays((int)currentDate.DayOfWeek == 0 ? -7 : -(int)currentDate.DayOfWeek);
                schedules = _scheduleRepository.Entities
                .Where(x => !x.IsDeleted && x.StartTime.Date > firstDateOfThisWeek.Date && x.StartTime.Date <= currentDate.Date);
            }
            else if (request.TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDayOfThisMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                schedules = _scheduleRepository.Entities
                    .Where(x => !x.IsDeleted && x.StartTime.Date >= firstDayOfThisMonth.Date && x.StartTime.Date <= currentDate.Date);
            }
            else if (request.TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDayOfThisYear = new DateTime(currentDate.Year, 1, 1);
                schedules = _scheduleRepository.Entities
                    .Where(x => !x.IsDeleted && x.StartTime.Date >= firstDayOfThisYear.Date && x.StartTime.Date <= currentDate.Date);
            }
            else
            {
                DateTime toTime = request.ToTime.HasValue ? request.ToTime.Value : currentDate;
                schedules = _scheduleRepository.Entities
                .Where(x => !x.IsDeleted
                && (request.FromTime.HasValue && x.StartTime.Date >= request.FromTime.Value.Date && x.StartTime.Date <= toTime.Date));
            }
            return schedules;
        }
    }
}
