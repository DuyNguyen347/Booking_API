using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Features.Schedule.Query.GetById;
using Application.Interfaces;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Review;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Domain.Wrappers;
using MediatR;


namespace Application.Features.Schedule.Query.GetAll.GetAllSchedule
{
    public class GetAllScheduleQuery: IRequest<Result<List<GetAllScheduleResponse>>>
    {
        public long? CinemaId {  get; set; }
    }
    internal class GetAllScheduleHandler : IRequestHandler<GetAllScheduleQuery, Result<List<GetAllScheduleResponse>>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUploadService _uploadService;
        private readonly IFilmImageRepository _filmImageRepository;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IReviewRepository _reviewRepository;
        public GetAllScheduleHandler(
            IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            IUploadService uploadService,
            IFilmImageRepository filmImageRepository,
            ITimeZoneService timeZoneService,
            IReviewRepository reviewRepository)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _uploadService = uploadService;
            _filmImageRepository = filmImageRepository;
            _timeZoneService = timeZoneService;
            _reviewRepository = reviewRepository;
        }
        public async Task<Result<List<GetAllScheduleResponse>>> Handle(GetAllScheduleQuery request, CancellationToken cancellationToken)
        {
            var availableToBookTime = _timeZoneService.GetGMT7Time().AddMinutes(15);

            var result = new List<GetAllScheduleResponse>();

            var query = (from cinema in _cinemaRepository.Entities 
                         where !cinema.IsDeleted && (request.CinemaId == null || cinema.Id == request.CinemaId)
                         join room in _roomRepository.Entities
                         on new { CinemaId = cinema.Id, IsDeleted = false } equals new { room.CinemaId, room.IsDeleted }
                         into cinema_room
                         from cr in cinema_room.DefaultIfEmpty()
                         join schedule in _scheduleRepository.Entities
                         on new { RoomId = cr.Id, IsDeleted = false} equals new { schedule.RoomId, schedule.IsDeleted }
                         into cinema_room_schedule
                         from crs in cinema_room_schedule.DefaultIfEmpty()
                         join film in _filmRepository.Entities
                         on new { Id = crs.FilmId, IsDeleted = false} equals new { film.Id, film.IsDeleted}
                         into film_group
                         from film in film_group.DefaultIfEmpty()
                         select new
                         {
                             cinemaInfo = cinema,
                             room = cr,
                             schedule = crs,
                             filmInfo = film
                         });
            var queryGroupByCinema = query.GroupBy(x => x.cinemaInfo.Id);
            foreach (var cinemaGroup in queryGroupByCinema)
            {
                var cinema = new GetAllScheduleResponse()
                {
                    CinemaId = cinemaGroup.Key,
                    City = cinemaGroup.First().cinemaInfo.City,
                    Name = cinemaGroup.First().cinemaInfo.Name,
                    Films = new List<GetAllScheduleFilmResponse>()
                };
                var groupByFilm = cinemaGroup.Where(x => x.filmInfo != null).GroupBy(x => x.filmInfo.Id);
                foreach (var filmGroup in groupByFilm)
                {
                    var film = new GetAllScheduleFilmResponse()
                    {
                        Id = filmGroup.Key,
                        Name = filmGroup.First().filmInfo.Name,
                        Duration = filmGroup.First().filmInfo.Duration,
                        Description = filmGroup.First().filmInfo.Description,
                        LimitAge = filmGroup.First().filmInfo.LimitAge,
                        Actor = filmGroup.First().filmInfo.Actor,
                        Director = filmGroup.First().filmInfo.Director,
                        Producer = filmGroup.First().filmInfo.Producer,
                        Country = filmGroup.First().filmInfo.Country,
                        Year = filmGroup.First().filmInfo.Year,
                        StartDate = filmGroup.First().filmInfo.StartDate,
                        EndDate = filmGroup.First().filmInfo.EndDate,
                        Trailer = filmGroup.First().filmInfo.Trailer,
                        Image = _filmRepository.GetImage(filmGroup.First().filmInfo.Id),
                        NumberOfVotes = _reviewRepository.GetFilmNumberOfReviews(filmGroup.First().filmInfo.Id),
                        Score = _reviewRepository.GetFilmReviewScore(filmGroup.First().filmInfo.Id),
                        Schedules = new List<GetAllScheduleScheduleResponse>()
                    };
                    foreach (var scheduleInfo in filmGroup)
                    {
                        if (scheduleInfo.schedule.StartTime > availableToBookTime && _scheduleRepository.IsBookableSchedule(scheduleInfo.schedule.Id))
                        {
                            film.Schedules.Add(new GetAllScheduleScheduleResponse()
                            {
                                Id = scheduleInfo.schedule.Id,
                                Duration = scheduleInfo.schedule.Duration,
                                StartTime = scheduleInfo.schedule.StartTime,
                                EndTime = scheduleInfo.schedule.StartTime.AddMinutes(film.Duration),
                                RoomId = scheduleInfo.schedule.RoomId,
                                Price = scheduleInfo.schedule.Price
                            });
                        }
                    }
                    if (film.Schedules.Count() > 0) 
                        cinema.Films.Add(film);
                }
                result.Add(cinema);
            }
            return await Result<List<GetAllScheduleResponse>>.SuccessAsync(result);
        }
    }
}
