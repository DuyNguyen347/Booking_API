using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Seat;
using Application.Interfaces.Services;
using Application.Interfaces.Ticket;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatReservationService _seatReservationService;
        public GetScheduleByIdHandler(
            IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            ISeatRepository seatRepository,
            ITicketRepository ticketRepository,
            ISeatReservationService seatReservationService)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _seatRepository = seatRepository;
            _ticketRepository = ticketRepository;
            _seatReservationService = seatReservationService;
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
            if (schedule == null) return await Result<GetScheduleByIdResponse>.FailAsync("NOT_FOUND_SCHEDULE");

            HashSet<int> lockedSeats = _seatReservationService.GetLockedSeats(request.Id);
            List<GetScheduleByIdSeatResponse> scheduleSeats = await _seatRepository.Entities
                .Where(x => x.RoomId == schedule.RoomId)
                .Select(x => new GetScheduleByIdSeatResponse
                {
                    Id = x.Id,
                    NumberSeat = x.NumberSeat,
                    SeatCode = x.SeatCode,
                    Status = lockedSeats.Contains(x.NumberSeat)?SeatStatus.Reserved:SeatStatus.Available
                }).ToListAsync();
            schedule.scheduleSeats = scheduleSeats;
            return await Result<GetScheduleByIdResponse>.SuccessAsync(schedule);
        }
    }
}
