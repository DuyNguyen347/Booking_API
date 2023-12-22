using Application.Features.Review.Query;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Review;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Application.Interfaces.Ticket;
using Application.Parameters;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Runtime.Serialization;

namespace Application.Features.Statistics.Queries.GetFilmStatistic
{
    public class GetFilmStatisticQuery: RequestParameter, IRequest<PaginatedResult<GetFilmStatisticResponse>>
    {
        [Required]
        public StatisticsTimeOption TimeOption { get; set; }
        public DateTime? FromTime {  get; set; }
        public DateTime? ToTime {  get; set; }
        public long FilmId { get; set; }
    }
    public class GetRevenueByFilmQueryHandler : IRequestHandler<GetFilmStatisticQuery, PaginatedResult<GetFilmStatisticResponse>>
    {
        private readonly IEnumService _enumService;
        private readonly IFilmRepository _filmRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IReviewRepository _reviewRepository;
        public GetRevenueByFilmQueryHandler(
            IEnumService enumService,
            IFilmRepository filmRepository,
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ITimeZoneService timeZoneService,
            IReviewRepository reviewRepository)
        {
            _enumService = enumService;
            _filmRepository = filmRepository;
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _timeZoneService = timeZoneService;
            _reviewRepository = reviewRepository;
        }
        public async Task<PaginatedResult<GetFilmStatisticResponse>> Handle(GetFilmStatisticQuery request, CancellationToken cancellationToken)
        {
            var filmStatistics = new List<GetFilmStatisticResponse>();
            DateTime currentDate = _timeZoneService.GetGMT7Time();
            IEnumerable<Domain.Entities.Booking.Booking> bookings;

            //Lay danh sach booking theo thoi gian
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
            var query = (from film in _filmRepository.Entities
                         where !film.IsDeleted && (request.FilmId == 0 || film.Id == request.FilmId)
                         join schedule in _scheduleRepository.Entities
                         on new { FilmId = film.Id, IsDeleted = false } equals new { schedule.FilmId, schedule.IsDeleted }
                         into filmSchedules
                         from filmSchedule in filmSchedules.DefaultIfEmpty()
                         join booking in _bookingRepository.Entities
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

                decimal revenue = 0;
                int numberOfTickets = 0;
                foreach (var booking in filmBooking)
                {
                    if (booking.bookingInfo!=null) 
                    {
                        revenue += booking.bookingInfo.RequiredAmount.HasValue ? booking.bookingInfo.RequiredAmount.Value : 0;
                        numberOfTickets += _bookingRepository.GetBookingNumberOfTickets(booking.bookingInfo.Id);
                    }
                }
                filmStatistic.Revenue = revenue;
                filmStatistic.NumberOfTickets = numberOfTickets;
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
                data = data.OrderBy("Revenue desc");

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
