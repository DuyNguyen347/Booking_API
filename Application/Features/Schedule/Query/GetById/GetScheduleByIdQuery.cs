using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.ScheduleSeat;
using Application.Interfaces.Seat;
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
        private readonly ISeatRepository _seatRepository;
        private readonly IScheduleSeatRepository _scheduleSeatRepository;
        public GetScheduleByIdHandler(
            IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            ISeatRepository seatRepository,
            IScheduleSeatRepository scheduleSeatRepository)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _seatRepository = seatRepository;
            _scheduleSeatRepository = scheduleSeatRepository;
        }
        public async Task<Result<GetScheduleByIdResponse>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (existSchedule == null) return await Result<GetScheduleByIdResponse>.FailAsync("NOT_FOUND_SCHEDULE");
            var schedule = await (from s in _scheduleRepository.Entities
                                   join room in _roomRepository.Entities on s.RoomId equals room.Id
                                   join cinema in _cinemaRepository.Entities on room.CinemaId equals cinema.Id
                                   join film in _filmRepository.Entities on s.FilmId equals film.Id
                                   where s.Id == request.Id 
                                   where !room.IsDeleted && !cinema.IsDeleted && !film.IsDeleted
                                   select new GetScheduleByIdResponse
                                   {
                                       Id = s.Id,
                                       Duration = s.Duration,
                                       Description = s.Description,
                                       StartTime = s.StartTime,
                                       EndTime = s.StartTime.AddMinutes(s.Duration),
                                       FilmId = film.Id,
                                       CinemaId = cinema.Id,
                                       RoomId = room.Id,
                                       Price = s.Price                                  
                                   }).FirstOrDefaultAsync();
            var seatSchedule = await (from scheduleSeat in _scheduleSeatRepository.Entities
                                      join seat in _seatRepository.Entities on scheduleSeat.SeatId equals seat.Id
                                      where !seat.IsDeleted && !scheduleSeat.IsDeleted
                                      select new GetScheduleByIdSeatResponse
                                      {
                                          Id = scheduleSeat.Id,
                                          SeatId = seat.Id,
                                          NumberSeat = seat.NumberSeat,
                                          SeatCode = seat.SeatCode,
                                          Status = scheduleSeat.Status
                                      }).ToListAsync(cancellationToken:cancellationToken);
            schedule.scheduleSeats = seatSchedule;
            if (schedule == null) return await Result<GetScheduleByIdResponse>.FailAsync("NOT_FOUND_SCHEDULE");
            return await Result<GetScheduleByIdResponse>.SuccessAsync(schedule);
        }
    }
}
