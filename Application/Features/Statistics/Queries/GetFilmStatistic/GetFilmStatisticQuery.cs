using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Review;
using Application.Interfaces.Schedule;
using Application.Parameters;
using Domain.Constants.Enum;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace Application.Features.Statistics.Queries.GetFilmStatistic
{
    public class GetFilmStatisticQuery : RequestParameter, IRequest<PaginatedResult<GetFilmStatisticResponse>>
    {
        [Required]
        public StatisticsTimeOption TimeOption { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public long FilmId { get; set; }
        public long CinemaId { get; set; }
    }
    public class GetFilmStatisticQueryHandler : IRequestHandler<GetFilmStatisticQuery, PaginatedResult<GetFilmStatisticResponse>>
    {
        private readonly IFilmRepository _filmRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IReviewRepository _reviewRepository;
        public GetFilmStatisticQueryHandler(
            IFilmRepository filmRepository,
            IBookingRepository bookingRepository,
            ICinemaRepository cinemaRepository,
            IScheduleRepository scheduleRepository,
            IReviewRepository reviewRepository)
        {
            _filmRepository = filmRepository;
            _bookingRepository = bookingRepository;
            _cinemaRepository = cinemaRepository;
            _scheduleRepository = scheduleRepository;
            _reviewRepository = reviewRepository;
        }
        public async Task<PaginatedResult<GetFilmStatisticResponse>> Handle(GetFilmStatisticQuery request, CancellationToken cancellationToken)
        {
            var filmStatistics = new List<GetFilmStatisticResponse>();
            if (request.CinemaId != 0 && !_cinemaRepository.Entities.Any(_ => !_.IsDeleted && _.Id == request.CinemaId))
                return PaginatedResult<GetFilmStatisticResponse>.Failure(new List<string> { "NOT_FOUND_CINEMA" });

            var bookings = _bookingRepository
            .GetCurrPrdBookingsByTimeChoice(request.TimeOption, request.FromTime, request.ToTime, request.CinemaId)
            .AsQueryable();

            //Thuc hien loc ket qua
            var query = (from film in _filmRepository.Entities
                         where !film.IsDeleted && (request.FilmId == 0 || film.Id == request.FilmId)
                         join schedule in _scheduleRepository.Entities
                         on new { FilmId = film.Id, IsDeleted = false } equals new { schedule.FilmId, schedule.IsDeleted }
                         into filmSchedules
                         from filmSchedule in filmSchedules.DefaultIfEmpty()
                         join booking in bookings
                         on filmSchedule.Id equals booking.ScheduleId
                         into filmBookings
                         from filmBooking in filmBookings.DefaultIfEmpty()
                         select new
                         {
                             filmInfo = film,
                             bookingInfo = filmBooking
                         });

            var queryGroupByFilm = query.GroupBy(x => x.filmInfo.Id);

            foreach (var filmBooking in queryGroupByFilm)
            {
                var filmStatistic = new GetFilmStatisticResponse
                {
                    Id = filmBooking.First().filmInfo.Id,
                    Name = filmBooking.First().filmInfo.Name,
                    Category = _filmRepository.GetCategory(filmBooking.First().filmInfo.Id),
                    Duration = filmBooking.First().filmInfo.Duration,
                    Image = _filmRepository.GetImage(filmBooking.First().filmInfo.Id),
                    NumberOfVotes = _reviewRepository.GetFilmNumberOfReviews(filmBooking.First().filmInfo.Id),
                    Score = _reviewRepository.GetFilmReviewScore(filmBooking.First().filmInfo.Id)
                };

                decimal revenue = 0m;
                int numberOfTickets = 0;
                int numberOfBookings = 0;
                foreach (var booking in filmBooking)
                {
                    if (booking.bookingInfo != null)
                    {
                        revenue += booking.bookingInfo.RequiredAmount.HasValue ? booking.bookingInfo.RequiredAmount.Value : 0;
                        numberOfTickets += _bookingRepository.GetBookingNumberOfTickets(booking.bookingInfo.Id);
                        numberOfBookings += 1;
                    }
                }
                filmStatistic.TotalRevenue = revenue;
                filmStatistic.NumberOfTickets = numberOfTickets;
                filmStatistic.NumberOfBookings = numberOfBookings;
                filmStatistics.Add(filmStatistic);
            }

            //tim kiem va sap xep
            IEnumerable<GetFilmStatisticResponse> enumStatistics = filmStatistics;

            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var data = enumStatistics.Where(x => string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.Name, request.Keyword)
                                || StringHelper.Contains(x.Category, request.Keyword)).AsQueryable();

            var totalCount = data.Count();

            if (!request.OrderBy.Contains("CreatedOn"))
                data = data.OrderBy(request.OrderBy);
            else
                data = data.OrderBy("TotalRevenue desc");

            //Phan trang
            List<GetFilmStatisticResponse> result;
            if (!request.IsExport)
                result = data.Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize).ToList();
            else
                result = data.ToList();

            return PaginatedResult<GetFilmStatisticResponse>.Success(result, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
