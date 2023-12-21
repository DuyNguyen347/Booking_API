using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Query.GetAll.GetAll
{
    public class GetAllScheduleByFilmQuery : IRequest<Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>>
    {
        public long FilmId {  get; set; }
    }
    internal class GetAllScheduleByFilmHander : IRequestHandler<GetAllScheduleByFilmQuery, Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ITimeZoneService _timeZoneService;

        public GetAllScheduleByFilmHander(
            IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            ITimeZoneService timeZoneService)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _timeZoneService = timeZoneService;
        }
        public async Task<Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>> Handle(GetAllScheduleByFilmQuery request, CancellationToken cancellationToken)
        {
            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            if (existFilm == null) return await Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>.FailAsync("NOT_FOUND_FILM");
            var scheduleData = new Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>();
            var schedules = await (from schedule in _scheduleRepository.Entities
                                   join room in _roomRepository.Entities on schedule.RoomId equals room.Id
                                   join cinema in _cinemaRepository.Entities on room.CinemaId equals cinema.Id
                                   where schedule.FilmId == request.FilmId
                                   where !cinema.IsDeleted && !room.IsDeleted && !schedule.IsDeleted
                                   select new
                                   {
                                       scheduleInfo = schedule,
                                       cinemId = cinema.Id,
                                       city = cinema.City
                                   }).ToListAsync();
            var availableToBookTime = _timeZoneService.GetGMT7Time().AddMinutes(15);
            foreach (var schedule in schedules)
            {
                if (!scheduleData.ContainsKey(schedule.city))
                {
                    scheduleData[schedule.city] = new Dictionary<long, List<GetAllScheduleByFilmResponse>>();
                }
                if (!scheduleData[schedule.city].ContainsKey(schedule.cinemId))
                {
                    scheduleData[schedule.city][schedule.cinemId] = new List<GetAllScheduleByFilmResponse>();
                }
                if (schedule.scheduleInfo.StartTime > availableToBookTime && _scheduleRepository.IsBookableSchedule(schedule.scheduleInfo.Id))
                {
                    scheduleData[schedule.city][schedule.cinemId].Add(new GetAllScheduleByFilmResponse
                    {
                        Id = schedule.scheduleInfo.Id,
                        StartTime = schedule.scheduleInfo.StartTime,
                        EndTime = schedule.scheduleInfo.StartTime.AddMinutes(existFilm.Duration),
                        Price = schedule.scheduleInfo.Price,
                    });
                }
            };
            return await Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>.SuccessAsync(scheduleData);
        }
    }
}
