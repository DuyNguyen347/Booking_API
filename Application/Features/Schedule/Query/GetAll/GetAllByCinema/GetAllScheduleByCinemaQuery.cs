using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetAll.GetAllByCinema
{
    public class GetAllScheduleByCinemaQuery : IRequest<ScheduleResult<GetAllScheduleByCinemaResponse>>
    {
        public long CinemaId { get; set; }
    }
    internal class GetAllScheduleByCinemaHandler : IRequestHandler<GetAllScheduleByCinemaQuery, ScheduleResult<GetAllScheduleByCinemaResponse>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;

        public GetAllScheduleByCinemaHandler(IScheduleRepository scheduleRepository, IFilmRepository filmRepository, ICinemaRepository cinemaRepository, IRoomRepository roomRepository)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
        }
        public async Task<ScheduleResult<GetAllScheduleByCinemaResponse>> Handle(GetAllScheduleByCinemaQuery request, CancellationToken cancellationToken)
        {
            var existCinema = await _cinemaRepository.FindAsync(x => x.Id == request.CinemaId && !x.IsDeleted);
            if (existCinema == null) return await ScheduleResult<GetAllScheduleByCinemaResponse>.FailAsync("NOT_FOUND_CINEMA");
            var schedules = await (from schedule in _scheduleRepository.Entities
                                   join room in _roomRepository.Entities on schedule.RoomId equals room.Id
                                   join film in _filmRepository.Entities on schedule.FilmId equals film.Id
                                   where room.CinemaId == request.CinemaId
                                   select new GetAllScheduleByCinemaResponse
                                   {
                                       Id = schedule.Id,
                                       Duration = schedule.Duration,
                                       Description = schedule.Description,
                                       StartTime = schedule.StartTime,
                                       Film = film.Name,
                                       Room = room.Name,
                                       Price = schedule.Price,
                                   }).ToListAsync();
            
            return await ScheduleResult<GetAllScheduleByCinemaResponse>.SuccessAsync(schedules);
        }
    }
}
