using Application.Interfaces.Repositories;

namespace Application.Interfaces.Schedule
{
    public interface IScheduleRepository : IRepositoryAsync<Domain.Entities.Schedule.Schedule, long>
    {
        string? GetFilmName(long scheduleId);
        string? GetCinemaName(long scheduleId);
        bool IsBookableSchedule(long scheduleId);
    }
}
