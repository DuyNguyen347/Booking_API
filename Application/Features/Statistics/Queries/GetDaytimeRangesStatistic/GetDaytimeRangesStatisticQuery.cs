using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Schedule;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
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
        public long CinemaId {  get; set; }
        public string? OrderBy {  get; set; }
    }
    public class GetDaytimeRangesStatisticQueryHandler : IRequestHandler<GetDaytimeRangesStatisticQuery, Result<List<GetDaytimeRangesStatisticResponse>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IEnumService _enumService;
        
        public GetDaytimeRangesStatisticQueryHandler(
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ICinemaRepository cinemaRepository,
            IEnumService enumService)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _cinemaRepository = cinemaRepository;
            _enumService = enumService;
        }

        public async Task<Result<List<GetDaytimeRangesStatisticResponse>>> Handle(GetDaytimeRangesStatisticQuery request, CancellationToken cancellationToken)
        {
            if (request.CinemaId != 0 && !_cinemaRepository.Entities.Any(_ => !_.IsDeleted && _.Id == request.CinemaId))
                return Result<List<GetDaytimeRangesStatisticResponse>>.Fail("NOT_FOUND_CINEMA");

            var timeRangesStatistic = new List<GetDaytimeRangesStatisticResponse>
            {
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour =  8, 
                    EndHour =  12,
                    Revenue = 0m,
                    OccupancyRate = 0,
                },
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour =  12,
                    EndHour = 16,
                    Revenue = 0m,
                    OccupancyRate = 0
                },
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour = 16,
                    EndHour = 20,
                    Revenue = 0m,
                    OccupancyRate = 0
                },
                new GetDaytimeRangesStatisticResponse()
                {
                    StartHour = 20,
                    EndHour = 24,
                    Revenue = 0m,
                    OccupancyRate = 0
                }
            };

            var schedules = _scheduleRepository
                .GetCurrPrdScheduleByTimeOption(request.TimeOption, request.FromTime, request.ToTime, request.CinemaId)
                .AsQueryable();

            var scheduleBookingsQuery = from schedule in schedules
                                join booking in _bookingRepository.Entities
                                .Where(booking => !booking.IsDeleted 
                                && booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                                on schedule.Id equals booking.ScheduleId
                                into scheduleBookings
                                from scheduleBooking in scheduleBookings.DefaultIfEmpty()
                                select new
                                {
                                    scheduleInfo = schedule,
                                    bookingInfo = scheduleBooking
                                };
            var groupScheduleBookings = scheduleBookingsQuery.GroupBy(_ => _.scheduleInfo.Id);

            foreach (var groupSchedule in groupScheduleBookings)
            {
                var scheduleHour = groupSchedule.First().scheduleInfo.StartTime.Hour;
                var timeRange = timeRangesStatistic.FirstOrDefault(range =>
                    range.StartHour <= scheduleHour
                    && scheduleHour < range.EndHour
                );

                if ( timeRange != null)
                {
                    foreach (var booking in groupSchedule)
                    {
                        if (booking.bookingInfo != null)
                        {
                            timeRange.Revenue += booking.bookingInfo.RequiredAmount.HasValue ? booking.bookingInfo.RequiredAmount.Value : 0;
                            timeRange.NumberOfBookings += 1;
                            timeRange.NumberOfTickets += _bookingRepository.GetBookingNumberOfTickets(booking.bookingInfo.Id);
                        }
                    }
                    timeRange.NumberOfSchedules += 1;
                    timeRange.OccupancyRate += _scheduleRepository.GetOccupancyRate(groupSchedule.First().scheduleInfo.Id);
                }
            }

            foreach(var timeRange in timeRangesStatistic)
            {
                if (timeRange.NumberOfSchedules > 0)
                {
                    timeRange.OccupancyRate /= timeRange.NumberOfSchedules;
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
    }
}
