using Application.Interfaces.Repositories;
using Domain.Constants.Enum;

namespace Application.Interfaces.Booking
{
    public interface IBookingRepository : IRepositoryAsync<Domain.Entities.Booking.Booking, long>
    {
        decimal GetAllTotalMoneyBookingByCustomerId(long id);
        int GetBookingNumberOfTickets(long id);
        IEnumerable<Domain.Entities.Booking.Booking> GetCurrPrdBookingsByTimeChoice(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0);
        IEnumerable<Domain.Entities.Booking.Booking> GetPrevPrdBookingsByTimeChoice(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0);
    }
}
