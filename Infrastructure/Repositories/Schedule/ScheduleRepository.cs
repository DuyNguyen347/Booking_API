using Application.Features.Schedule.Query.GetById;
using Application.Interfaces.Schedule;
using Domain.Constants.Enum;
using Domain.Constants;
using Domain.Entities.Schedule;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Application.Interfaces.Booking;
using Infrastructure.Repositories.Booking;
using Infrastructure.Services;
using Application.Interfaces;

namespace Infrastructure.Repositories.Schedule
{
    public class ScheduleRepository : RepositoryAsync<Domain.Entities.Schedule.Schedule, long>, IScheduleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISeatReservationService _seatReservationService;
        private readonly IEnumService _enumService;
        private readonly ITimeZoneService _timeZoneService;
        public ScheduleRepository(
            ApplicationDbContext dbContext,
            ISeatReservationService seatReservationService,
            IEnumService enumService,
            ITimeZoneService timeZoneService) : base(dbContext)
        {
            _dbContext = dbContext;
            _seatReservationService = seatReservationService;
            _enumService = enumService;
            _timeZoneService = timeZoneService;
        }

        public string? GetFilmName(long scheduleId)
        {
            var filmName = (from schedule in _dbContext.Schedules
                            where schedule.Id == scheduleId && !schedule.IsDeleted
                            join film in _dbContext.Films
                            on new { Id = schedule.FilmId, IsDeleted = false } equals new { film.Id, film.IsDeleted }
                            select film.Name).FirstOrDefault();
            return filmName;
        }

        public string? GetCinemaName(long scheduleId)
        {
            var cinemaName = (from  schedule in _dbContext.Schedules
                              where schedule.Id != scheduleId && !schedule.IsDeleted
                              join room in _dbContext.Room
                              on new { Id = schedule.Id, IsDeleted=false } equals new { room.Id, room.IsDeleted }
                              join cinema in _dbContext.Cinemas
                              on new {Id = room.CinemaId, IsDeleted=false } equals new {cinema.Id, cinema.IsDeleted}
                              select cinema.Name).FirstOrDefault();
            return cinemaName;
        }

        public bool IsBookableSchedule(long scheduleId)
        {
            var schedule = _dbContext.Schedules.Where(s => s.Id == scheduleId && !s.IsDeleted).FirstOrDefault();
            HashSet<int> lockedSeats = _seatReservationService.GetLockedSeats(scheduleId);

            HashSet<int> bookedSeats = (from booking in _dbContext.Bookings
                                        where !booking.IsDeleted && booking.ScheduleId == scheduleId
                                        where booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                                        || booking.ExpireDate > _timeZoneService.GetGMT7Time()
                                        join ticket in _dbContext.Tickets
                                        on new { BookingId = booking.Id, IsDeleted = false }
                                        equals new { ticket.BookingId, ticket.IsDeleted }
                                        select ticket.NumberSeat).ToHashSet();
            var scheduleSeats = _dbContext.Seats
                .Count(x => x.RoomId == schedule.RoomId
                && !x.IsDeleted
                && !bookedSeats.Contains(x.NumberSeat)
                && !lockedSeats.Contains(x.NumberSeat));
            return scheduleSeats > 0 ? true : false;
        }
    }
}
