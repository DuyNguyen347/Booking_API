using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Seat;
using Application.Interfaces.Services;
using Application.Interfaces.Ticket;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Entities.Booking;
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
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatReservationService _seatReservationService;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ITimeZoneService _timeZoneService;
        public GetScheduleByIdHandler(
            IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            IBookingRepository bookingRepository,
            ISeatRepository seatRepository,
            ITicketRepository ticketRepository,
            ISeatReservationService seatReservationService,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork,
            ITimeZoneService timeZoneService)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
            _ticketRepository = ticketRepository;
            _seatReservationService = seatReservationService;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
            _timeZoneService = timeZoneService;
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
                                       Price = s.Price,
                                       CreatedOn = s.CreatedOn,
                                       LastModifiedOn = s.LastModifiedOn
                                   }).FirstOrDefaultAsync();
            if (schedule == null) return await Result<GetScheduleByIdResponse>.FailAsync("NOT_FOUND_SCHEDULE");


            HashSet<int> lockedSeats = _seatReservationService.GetLockedSeats(request.Id);

            HashSet<int> bookedSeats = (from booking in _bookingRepository.Entities
                                        where !booking.IsDeleted  && booking.ScheduleId == schedule.Id
                                        where booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                                        || booking.ExpireDate > _timeZoneService.GetGMT7Time() 
                                        join ticket in _ticketRepository.Entities
                                        on new { BookingId = booking.Id, IsDeleted = false } 
                                        equals new { ticket.BookingId, ticket.IsDeleted }
                                     select ticket.NumberSeat).ToHashSet();
            List<GetScheduleByIdSeatResponse> scheduleSeats = await _seatRepository.Entities
                .Where(x => x.RoomId == schedule.RoomId && !x.IsDeleted)
                .Select(x => new GetScheduleByIdSeatResponse
                {
                    Id = x.Id,
                    NumberSeat = x.NumberSeat,
                    SeatCode = x.SeatCode,
                    Status = bookedSeats.Contains(x.NumberSeat) ? SeatStatus.Disabled 
                    : (lockedSeats.Contains(x.NumberSeat) ? SeatStatus.Reserved 
                    : SeatStatus.Available)
                }).ToListAsync();
            schedule.scheduleSeats = scheduleSeats;
            return await Result<GetScheduleByIdResponse>.SuccessAsync(schedule);
        }
    }
}
