using Application.Interfaces.Repositories;
using Domain.Constants.Enum;

namespace Application.Interfaces.Booking
{
    public interface IBookingRepository : IRepositoryAsync<Domain.Entities.Booking.Booking, long>
    {
        decimal GetAllTotalMoneyBookingByCustomerId(long id);
        int GetBookingNumberOfTickets(long id);
        IEnumerable<Domain.Entities.Booking.Booking> GetBookingsByTimeChoice(StatisticsTimeOption timeOption, DateTime? FromTime, DateTime? ToTime);
    }
}
