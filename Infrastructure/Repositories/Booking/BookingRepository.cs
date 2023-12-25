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

        public IEnumerable<Domain.Entities.Booking.Booking> GetBookingsByTimeChoice(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime)
        {
            IEnumerable<Domain.Entities.Booking.Booking> bookings;

            DateTime currentDate = _timeZoneService.GetGMT7Time();

            if (TimeOption == StatisticsTimeOption.Today)
            {
                bookings = _dbContext.Bookings
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date == currentDate.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
            }
            else if ( TimeOption == StatisticsTimeOption.ThisWeek)
            {
                DateTime firstDateOfThisWeek = currentDate.AddDays((int)currentDate.DayOfWeek == 0 ? -7 : -(int)currentDate.DayOfWeek);
                bookings = _dbContext.Bookings
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date > firstDateOfThisWeek.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
            }
            else if ( TimeOption == StatisticsTimeOption.ThisMonth)
            {
                DateTime firstDayOfThisMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                bookings = _dbContext.Bookings
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date >= firstDayOfThisMonth.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
            }
            else if ( TimeOption == StatisticsTimeOption.ThisYear)
            {
                DateTime firstDayOfThisYear = new DateTime(currentDate.Year, 1, 1);
                bookings = _dbContext.Bookings
                    .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                    && x.BookingDate.Value.Date >= firstDayOfThisYear.Date
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
            }
            else
            {
                DateTime toTime =  ToTime.HasValue ?  ToTime.Value : currentDate;
                bookings = _dbContext.Bookings
                .Where(x => !x.IsDeleted && x.BookingDate.HasValue
                && (FromTime.HasValue && x.BookingDate.Value.Date >= FromTime.Value.Date && x.BookingDate.Value.Date <= toTime.Date)
                    && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM));
            }
            return bookings;
        }
        
        public IEnumerable<Domain.Entities.Booking.Booking> GetBookingsByCinema(long CinemaId)
        {
            var bookings = _dbContext.Bookings
                .Where(_ => !_.IsDeleted && _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
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
                (booking, room) => booking.bookingInfo).AsEnumerable();
            return bookings;
        }
    }
}