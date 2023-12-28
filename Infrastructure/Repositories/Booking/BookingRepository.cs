using Application.Interfaces.Booking;
using Infrastructure.Contexts;
using Domain.Constants;
using Application.Interfaces;
using Domain.Constants.Enum;
using Application.Interfaces.Services;

namespace Infrastructure.Repositories.Booking
{
    public class BookingRepository : RepositoryAsync<Domain.Entities.Booking.Booking, long>, IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEnumService _enumService;
        private readonly ITimeZoneService _timeZoneService;

        public BookingRepository(
            ApplicationDbContext dbContext,
            IEnumService enumService,
            ITimeZoneService timeZoneService) : base(dbContext)
        {
            _dbContext = dbContext;
            _enumService = enumService;
            _timeZoneService = timeZoneService;
        }

        public decimal GetAllTotalMoneyBookingByCustomerId(long id)
        {
            var totalMoney = _dbContext.Bookings.Where(b => b.CustomerId == id && !b.IsDeleted && b.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
               .Join(_dbContext.BookingDetails.Where(_ => !_.IsDeleted), b => b.Id, bd => bd.BookingId, (b, bd) => new { Booking = b, BookingDetail = bd })
             .Join(_dbContext.Services.Where(_ => !_.IsDeleted), bb => bb.BookingDetail.ServiceId, s => s.Id, (bb, s) => new { Booking = bb.Booking, Service = s })
             .GroupBy(bb => bb.Booking.CustomerId)
            .Select(group => new
            {
                CustomerId = group.Key,
                TotalMoney = group.Sum(bb => bb.Service.Price)
            })
            .FirstOrDefault(); ;
            return (totalMoney != null) ? totalMoney.TotalMoney : 0;
        }
        public int GetBookingNumberOfTickets(long id)
        {
            var numberOfTickets = _dbContext.Bookings.Where(booking => !booking.IsDeleted && booking.Id == id)
                .Join(_dbContext.Tickets.Where(ticket => !ticket.IsDeleted),
                booking => booking.Id, ticket => ticket.BookingId,
                (booking, ticket) => ticket).Count();
            return numberOfTickets;
        }

        public IEnumerable<Domain.Entities.Booking.Booking> GetCurrPrdBookingsByTimeChoice(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0)
        {
            IEnumerable<Domain.Entities.Booking.Booking> bookings;

            DateTime currentDate = _timeZoneService.GetGMT7Time();

            if (TimeOption == StatisticsTimeOption.Today)
            {
                bookings = GetBookingsByTimeRange(currentDate.Date, currentDate.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentDate.AddDays((int)currentDate.DayOfWeek == 0 ? -7 : -(int)currentDate.DayOfWeek);
                bookings = GetBookingsByTimeRange(firstDateOfThisWeek.Date, currentDate.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDateOfThisMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                bookings = GetBookingsByTimeRange(firstDateOfThisMonth.Date, currentDate.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDateOfThisYear = new DateTime(currentDate.Year, 1, 1);
                bookings = GetBookingsByTimeRange(firstDateOfThisYear.Date, currentDate.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.CustomTime)
            {
                bookings = GetBookingsByTimeRange(FromTime, ToTime, CinemaId);
            }
            else
            {
                bookings = GetBookingsByTimeRange(null, null, CinemaId);
            }

            return bookings;
        }

        public IEnumerable<Domain.Entities.Booking.Booking> GetPrevPrdBookingsByTimeChoice(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0)
        {
            IEnumerable<Domain.Entities.Booking.Booking> bookings;

            DateTime currentTime = _timeZoneService.GetGMT7Time();

            if (TimeOption == StatisticsTimeOption.Today)
            {
                DateTime prevDate = currentTime.AddDays(-1);
                bookings = GetBookingsByTimeRange(prevDate.Date, prevDate.Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentTime.AddDays((int)currentTime.DayOfWeek == 0 ? -7 : -(int)currentTime.DayOfWeek);
                DateTime firstDateOfLastWeek = firstDateOfThisWeek.AddDays(-7);
                bookings = GetBookingsByTimeRange(firstDateOfLastWeek.Date, firstDateOfThisWeek.AddDays(-1).Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDateOfThisMonth = new DateTime(currentTime.Year, currentTime.Month, 1);
                DateTime firstDateOfLastMonth = firstDateOfThisMonth.AddMonths(-1);
                bookings = GetBookingsByTimeRange(firstDateOfLastMonth.Date, firstDateOfThisMonth.AddDays(-1).Date, CinemaId);
            }
            else if (TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDateOfThisYear = new DateTime(currentTime.Year, 1, 1);
                DateTime firstDateOfLastYear = new DateTime(currentTime.Year - 1, 1, 1);
                bookings = GetBookingsByTimeRange(firstDateOfLastYear.Date, firstDateOfThisYear.AddDays(-1).Date, CinemaId);
            }
            else
            {
                bookings = new List<Domain.Entities.Booking.Booking>();
            }

            return bookings;
        }

        public IEnumerable<Domain.Entities.Booking.Booking> GetBookingsByTimeRange(DateTime? FromTime, DateTime? ToTime, long CinemaId)
        {
            IEnumerable<Domain.Entities.Booking.Booking> bookings;
            DateTime currentTime = _timeZoneService.GetGMT7Time();
            DateTime toTime = ToTime.HasValue ? ToTime.Value : currentTime;
            if (!FromTime.HasValue)
            {
                bookings = _dbContext.Bookings
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && (x.BookingDate.Value.Date <= toTime.Date)
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
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

                bookings = _dbContext.Bookings
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && (x.BookingDate.Value.Date >= fromTime.Date && x.BookingDate.Value.Date <= toTime.Date)
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
            }

            if (CinemaId == 0)
                return bookings;

            IEnumerable<Domain.Entities.Booking.Booking> cinemaBookings = bookings.AsQueryable()
                .Join(_dbContext.Schedules.Where(_ => !_.IsDeleted),
                    booking => booking.ScheduleId,
                    schedule => schedule.Id,
                    (booking, schedule) => new
                    {
                        bookingInfo = booking,
                        roomId = schedule.RoomId
                    })
                .Join(_dbContext.Room.Where(_ => !_.IsDeleted && (CinemaId == 0 || _.CinemaId == CinemaId)),
                    booking => booking.roomId,
                    room => room.Id,
                    (booking, room) => booking.bookingInfo);

            return cinemaBookings;
        }
    }
}