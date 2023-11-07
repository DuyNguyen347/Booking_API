using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
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

namespace Application.Features.Schedule.Query.GetById
{
    public class GetScheduleByIdQuery : IRequest<Result<GetScheduleByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetScheduleByIdHandler : IRequestHandler<GetScheduleByIdQuery, Result<GetScheduleByIdResponse>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;
        public GetScheduleByIdHandler(IScheduleRepository scheduleRepository, IFilmRepository filmRepository, ICinemaRepository cinemaRepository, IRoomRepository roomRepository)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
        }
        public async Task<Result<GetScheduleByIdResponse>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            var schedule = await (from s in _scheduleRepository.Entities
                                   join room in _roomRepository.Entities on s.RoomId equals room.Id
                                   join cinema in _cinemaRepository.Entities on room.CinemaId equals cinema.Id
                                   join film in _filmRepository.Entities on s.FilmId equals film.Id
                                   where s.Id == request.Id && !s.IsDeleted
                                   select new GetScheduleByIdResponse
                                   {
                                       Id = s.Id,
                                       Duration = s.Duration,
                                       Description = s.Description,
                                       StartTime = s.StartTime,
                                       Film = film.Name,
                                       Cinema = cinema.Name,
                                       Room = room.Name,
                                       Price = s.Price                                  
                                   }).FirstOrDefaultAsync(cancellationToken:cancellationToken);
            if (schedule == null) return await Result<GetScheduleByIdResponse>.FailAsync("NOT_FOUND_SCHEDULE");
            return await Result<GetScheduleByIdResponse>.SuccessAsync(schedule);
        }
    }
}
