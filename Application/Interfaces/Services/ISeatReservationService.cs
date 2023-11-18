using Application.Dtos.Requests;
using Application.Dtos.Responses;

namespace Application.Interfaces.Services
{
    public interface ISeatReservationService
    {
        bool LockSeats(long CustomerId, long ScheduleId, List<int> NumberSeats);
        void UnlockSeats(long CustomerId, long ScheduleId, List<int> NumberSeats);
        HashSet<int> GetLockedSeats(long ScheduleId);
        Dictionary<long, Dictionary<int, AddSeatReservationResponse>> GetAll();
    }
}
