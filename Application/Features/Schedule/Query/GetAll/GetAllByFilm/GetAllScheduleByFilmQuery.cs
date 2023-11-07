using Application.Features.Category.Queries.GetAll;
using Application.Features.Film.Queries.GetAll;
using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Interfaces.Category;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Domain.Entities.Schedule;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Query.GetAll.GetAll
{
    public class GetAllScheduleByFilmQuery : IRequest<Result<Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>>>
    {
        public long FilmId {  get; set; }
    }
    internal class GetAllScheduleHandler : IRequestHandler<GetAllScheduleByFilmQuery, Result<Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;

public GetAllScheduleHandler(IScheduleRepository scheduleRepository, IFilmRepository filmRepository, ICinemaRepository cinemaRepository, IRoomRepository roomRepository)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
        }
        public async Task<Result<Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>>> Handle(GetAllScheduleByFilmQuery request, CancellationToken cancellationToken)
        {
            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            if (existFilm == null) return await Result<Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>>.FailAsync("NOT_FOUND_FILM");
            var scheduleData = new Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>();
            var schedules = await (from schedule in _scheduleRepository.Entities
                                   join room in _roomRepository.Entities on schedule.RoomId equals room.Id
                                   join cinema in _cinemaRepository.Entities on room.CinemaId equals cinema.Id
                                   where schedule.FilmId == request.FilmId
                                   select new
                                   {
                                       schedule = schedule,
                                       roomName = room.Name,
                                       cinemaName = cinema.Name,
                                       city = cinema.City
                                   }).ToListAsync();
            foreach (var schedule in schedules)
            {
                if (!scheduleData.ContainsKey(schedule.city))
                {
                    scheduleData[schedule.city] = new Dictionary<string, List<GetAllScheduleByFilmResponse>>();
                }
                if (!scheduleData[schedule.city].ContainsKey(schedule.cinemaName))
                {
                    scheduleData[schedule.city][schedule.cinemaName] = new List<GetAllScheduleByFilmResponse>();
                }
                scheduleData[schedule.city][schedule.cinemaName].Add(new GetAllScheduleByFilmResponse
                {
                    Id = schedule.schedule.Id,
                    StartTime = schedule.schedule.StartTime
                });
            };
            return await Result<Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>>.SuccessAsync(scheduleData);
        }
    }
}
