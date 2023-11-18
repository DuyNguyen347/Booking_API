using Application.Dtos.Requests;

namespace Application.Interfaces.Services
{
    public interface ISeatReservationService
    {
        bool LockSeats(long CustomerId, long ScheduleId, List<int> NumberSeats);
        void UnlockSeats(long CustomerId, long ScheduleId, List<int> NumberSeats);
        HashSet<int> GetLockedSeats(long ScheduleId);
    }
}
