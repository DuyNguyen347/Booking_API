using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Features.Schedule.Query.GetById;
using Application.Interfaces;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
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
        public GetAllScheduleHandler(IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            IUploadService uploadService,
            IFilmImageRepository filmImageRepository,
            ITimeZoneService timeZoneService)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _uploadService = uploadService;
            _filmImageRepository = filmImageRepository;
            _timeZoneService = timeZoneService;
        }
        public async Task<Result<List<GetAllScheduleResponse>>> Handle(GetAllScheduleQuery request, CancellationToken cancellationToken)
        {
            var availableToBookTime = _timeZoneService.GetGMT7Time().AddMinutes(30);
            var result = new List<GetAllScheduleResponse>();
            var availableSchedule = (from schedule in _scheduleRepository.Entities
                                     where schedule.StartTime > availableToBookTime
                                     select schedule);
            var query = (from cinema in _cinemaRepository.Entities 
                         where !cinema.IsDeleted && (request.CinemaId == null || cinema.Id == request.CinemaId)
                         join room in _roomRepository.Entities
                         on new { CinemaId = cinema.Id, IsDeleted = false } equals new { room.CinemaId, room.IsDeleted }
                         into cinema_room
                         from cr in cinema_room.DefaultIfEmpty()
                         join schedule in availableSchedule
                         on new { RoomId = cr.Id, IsDeleted = false} equals new { schedule.RoomId, schedule.IsDeleted }
                         into cinema_room_schedule
                         from crs in cinema_room_schedule.DefaultIfEmpty()
                         join film in _filmRepository.Entities
                         on new { Id = crs.FilmId, IsDeleted = false} equals new { film.Id, film.IsDeleted}
                         into film_group
                         from film in film_group.DefaultIfEmpty()
                         select new
                         {
                             cinema = cinema,
                             room = cr,
                             schedule = crs,
                             film = film
                         });
            var queryGroupByCinema = query.GroupBy(x => x.cinema.Id);
            foreach (var cinemaGroup in queryGroupByCinema)
            {
                var cinema = new GetAllScheduleResponse()
                {
                    CinemaId = cinemaGroup.Key,
                    City = cinemaGroup.First().cinema.City,
                    Name = cinemaGroup.First().cinema.Name,
                    Films = new List<GetAllScheduleFilmResponse>()
                };
                var groupByFilm = cinemaGroup.Where(x => x.film != null).GroupBy(x => x.film.Id);
                foreach (var filmGroup in groupByFilm)
                {
                    var film = new GetAllScheduleFilmResponse()
                    {
                        Id = filmGroup.Key,
                        Name = filmGroup.First().film.Name,
                        Duration = filmGroup.First().film.Duration,
                        Description = filmGroup.First().film.Description,
                        LimitAge = filmGroup.First().film.LimitAge,
                        Actor = filmGroup.First().film.Actor,
                        Director = filmGroup.First().film.Director,
                        Producer = filmGroup.First().film.Producer,
                        Country = filmGroup.First().film.Country,
                        Year = filmGroup.First().film.Year,
                        StartDate = filmGroup.First().film.StartDate,
                        EndDate = filmGroup.First().film.EndDate,
                        Trailer = filmGroup.First().film.Trailer,
                        Image = _uploadService.GetFullUrl(_filmImageRepository.Entities.Where(_ => !_.IsDeleted && _.FilmId == filmGroup.First().film.Id).Select(y => y.NameFile).FirstOrDefault()),
                        Schedules = new List<GetAllScheduleScheduleResponse>()
                    };
                    foreach (var scheduleInfo in filmGroup)
                    {
                        var schedule = new GetAllScheduleScheduleResponse()
                        {
                            Id = scheduleInfo.schedule.Id,
                            Duration = scheduleInfo.schedule.Duration,
                            StartTime = scheduleInfo.schedule.StartTime,
                            EndTime = scheduleInfo.schedule.StartTime.AddMinutes(scheduleInfo.schedule.Duration),
                            RoomId = scheduleInfo.schedule.RoomId,
                            Price = scheduleInfo.schedule.Price
                        };
                        film.Schedules.Add(schedule);
                    }
                    cinema.Films.Add(film);
                }
                result.Add(cinema);
            }
            return await Result<List<GetAllScheduleResponse>>.SuccessAsync(result);
        }
    }
}
