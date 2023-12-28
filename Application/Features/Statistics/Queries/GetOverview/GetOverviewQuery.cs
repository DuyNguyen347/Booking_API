using Application.Interfaces.Booking;
using Domain.Wrappers;
using Domain.Constants.Enum;
using MediatR;
using Application.Interfaces.Schedule;
using Application.Interfaces.Cinema;

namespace Application.Features.Statistics.Queries.GetOverview
{
    public class GetOverviewQuery : IRequest<Result<GetOverviewResponse>>
    {
        public StatisticsTimeOption TimeOption { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public long CinemaId { get; set; }
    }
    internal class GetOverviewQueryHandler : IRequestHandler<GetOverviewQuery, Result<GetOverviewResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICinemaRepository _cinemaRepository;

        public GetOverviewQueryHandler(
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ICinemaRepository cinemaRepository)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _cinemaRepository = cinemaRepository;
        }

        public async Task<Result<GetOverviewResponse>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            if (request.CinemaId != 0 && !_cinemaRepository.Entities.Any(_ => !_.IsDeleted && _.Id == request.CinemaId))
                return Result<GetOverviewResponse>.Fail("NOT_FOUND_CINEMA");

            GetOverviewResponse result = new GetOverviewResponse()
            {
                CurrPrdTotalRevenue = 0m,
                PrevPrdTotalRevenue = 0m,
                CurrPrdOccupancyRate = 0,
                PrevPrdOccupancyRate = 0
            };

            var currPrdBookings = _bookingRepository
                .GetCurrPrdBookingsByTimeChoice(request.TimeOption, request.FromTime, request.ToTime, request.CinemaId)
                .AsQueryable();
            var prevPrdBookings = _bookingRepository
                .GetPrevPrdBookingsByTimeChoice(request.TimeOption, request.FromTime, request.ToTime, request.CinemaId)
                .AsQueryable();

            foreach (var booking in currPrdBookings)
            {
                result.CurrPrdTotalRevenue += booking.RequiredAmount.HasValue ? booking.RequiredAmount.Value : 0;
                result.CurrPrdTotalBookings += 1;
                result.CurrPrdTotalTickets += _bookingRepository.GetBookingNumberOfTickets(booking.Id);
            }
            foreach (var booking in prevPrdBookings)
            {
                result.PrevPrdTotalRevenue += booking.RequiredAmount.HasValue ? booking.RequiredAmount.Value : 0;
                result.PrevPrdTotalBookings += 1;
                result.PrevPrdTotalTickets += _bookingRepository.GetBookingNumberOfTickets(booking.Id);
            }

            var currPrdScheduleQuery = _scheduleRepository
                .GetCurrPrdScheduleByTimeOption(request.TimeOption, request.FromTime, request.ToTime, request.CinemaId)
                .AsQueryable();
            var prevPrdScheduleQuery = _scheduleRepository
                .GetPrevPrdScheduleByTimeOption(request.TimeOption, request.FromTime, request.ToTime, request.CinemaId)
                .AsQueryable();

            foreach (var schedule in currPrdScheduleQuery)
            {
                result.CurrPrdSchedules += 1;
                result.CurrPrdOccupancyRate += _scheduleRepository.GetOccupancyRate(schedule.Id);
            }
            foreach (var schedule in prevPrdScheduleQuery)
            {
                result.PrevPrdSchedules += 1;
                result.PrevPrdOccupancyRate += _scheduleRepository.GetOccupancyRate(schedule.Id);
            }

            if (result.CurrPrdSchedules > 0)
                result.CurrPrdOccupancyRate /= result.CurrPrdSchedules;
            if (result.PrevPrdSchedules > 0)
                result.PrevPrdOccupancyRate /= result.PrevPrdSchedules;

            return await Result<GetOverviewResponse>.SuccessAsync(result);
        }
    }
}
