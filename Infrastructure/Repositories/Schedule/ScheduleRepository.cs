using Application.Interfaces.Schedule;
using Domain.Constants;
using Infrastructure.Contexts;
using Application.Interfaces.Services;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Domain.Constants.Enum;

namespace Infrastructure.Repositories.Schedule
{
    public class ScheduleRepository : RepositoryAsync<Domain.Entities.Schedule.Schedule, long>, IScheduleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatReservationService _seatReservationService;
        private readonly IEnumService _enumService;
        private readonly ITimeZoneService _timeZoneService;
        public ScheduleRepository(
            ApplicationDbContext dbContext,
            IBookingRepository bookingRepository,
            ISeatReservationService seatReservationService,
            IEnumService enumService,
            ITimeZoneService timeZoneService) : base(dbContext)
        {
            _dbContext = dbContext;
            _bookingRepository = bookingRepository;
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
                              where schedule.Id == scheduleId && !schedule.IsDeleted
                              join room in _dbContext.Room
                              on new { Id = schedule.RoomId, IsDeleted=false } equals new { room.Id, room.IsDeleted }
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

        public decimal? GetOccupancyRate(long scheduleId)
        {
            var scheduleSeats = _dbContext.Schedules.Where(_ => !_.IsDeleted && _.Id == scheduleId)
                .Join(_dbContext.Room.Where(_ => !_.IsDeleted),
                schedule => schedule.RoomId,
                room => room.Id,
                (schedule, room) => room.NumberSeat).FirstOrDefault();

            var bookings = _dbContext.Bookings
                .Where(_ => !_.IsDeleted
                && _.ScheduleId == scheduleId
                && _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)).ToList();
            var numberOfTickets = bookings.Sum(booking => _bookingRepository.GetBookingNumberOfTickets(booking.Id));

            var rate = scheduleSeats > 0 ? (decimal)numberOfTickets / scheduleSeats : 0;
            return rate;
        }
        public IEnumerable<Domain.Entities.Schedule.Schedule> GetCurrPrdScheduleByTimeOption(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0)
        {
            IEnumerable<Domain.Entities.Schedule.Schedule> schedules;

            DateTime currentTime = _timeZoneService.GetGMT7Time();

            if (TimeOption == StatisticsTimeOption.Today)
            {
                schedules = GetScheduleByTimeRange(currentTime.Date, currentTime, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentTime.AddDays((int)currentTime.DayOfWeek == 0 ? -7 : -(int)currentTime.DayOfWeek);
                schedules = GetScheduleByTimeRange(firstDateOfThisWeek.Date, currentTime, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDateOfThisMonth = new DateTime(currentTime.Year, currentTime.Month, 1);
                schedules = GetScheduleByTimeRange(firstDateOfThisMonth.Date, currentTime, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDateOfThisYear = new DateTime(currentTime.Year, 1, 1);
                schedules = GetScheduleByTimeRange(firstDateOfThisYear.Date, currentTime, CinemaId);
            }
            else if  (TimeOption == StatisticsTimeOption.CustomTime)
            {
                schedules = GetScheduleByTimeRange(FromTime, ToTime, CinemaId);
            }
            else
            {
                schedules = GetScheduleByTimeRange(null, currentTime, CinemaId);
            }
            return schedules;
        }

        public IEnumerable<Domain.Entities.Schedule.Schedule> GetPrevPrdScheduleByTimeOption(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0)
        {
            IEnumerable<Domain.Entities.Schedule.Schedule> schedules;

            DateTime currentTime = _timeZoneService.GetGMT7Time();

            if (TimeOption == StatisticsTimeOption.Today)
            {
                DateTime prevDate = currentTime.AddDays(-1);
                schedules = GetScheduleByTimeRange(prevDate.Date, currentTime.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentTime.AddDays((int)currentTime.DayOfWeek == 0 ? -7 : -(int)currentTime.DayOfWeek);
                DateTime firstDateOfLastWeek = firstDateOfThisWeek.AddDays(-7);
                schedules = GetScheduleByTimeRange(firstDateOfLastWeek.Date, firstDateOfThisWeek.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDateOfThisMonth = new DateTime(currentTime.Year, currentTime.Month, 1);
                DateTime firstDateOfLastMonth = firstDateOfThisMonth.AddMonths(-1);
                schedules = GetScheduleByTimeRange(firstDateOfLastMonth.Date, firstDateOfThisMonth.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDateOfThisYear = new DateTime(currentTime.Year, 1, 1);
                DateTime firstDateOfLastYear = new DateTime(currentTime.Year - 1, 1, 1);
                schedules = GetScheduleByTimeRange(firstDateOfLastYear.Date, firstDateOfThisYear, CinemaId);
            }
            else
            {
                schedules = new List<Domain.Entities.Schedule.Schedule>();
            }
            return schedules;
        }

        public IEnumerable<Domain.Entities.Schedule.Schedule> GetScheduleByTimeRange(DateTime? FromTime,  DateTime? ToTime, long CinemaId)
        {
            IEnumerable<Domain.Entities.Schedule.Schedule> schedules;
            DateTime currentTime = _timeZoneService.GetGMT7Time();
            DateTime toTime = ToTime.HasValue ? (ToTime.Value < currentTime ? ToTime.Value : currentTime) :currentTime;
            if (!FromTime.HasValue)
            {
                schedules = _dbContext.Schedules
                    .Where(x => !x.IsDeleted && x.StartTime <= toTime);
            }
            else
            {
                DateTime fromTime = new DateTime();
                if (FromTime.Value < toTime)
                    fromTime = FromTime.Value;
                else if (FromTime.Value > currentTime)
                {
                    fromTime = toTime;
                    toTime = currentTime;
                }
                else
                {
                    DateTime tempTime = FromTime.Value;
                    fromTime = toTime;
                    toTime = tempTime;
                }
               
                schedules = _dbContext.Schedules
                    .Where(x => !x.IsDeleted
                    && (x.StartTime >= fromTime && x.StartTime <= toTime));
            }

            if (CinemaId == 0)
                return schedules;

            IEnumerable<Domain.Entities.Schedule.Schedule> cinemaSchedules = schedules.AsQueryable()
                .Join(_dbContext.Room.Where(_ => !_.IsDeleted && (CinemaId == 0 || _.CinemaId == CinemaId)),
                    schedule => schedule.RoomId,
                    room => room.Id,
                    (schedule, room) => schedule);

            return cinemaSchedules;
        }
    }
}
