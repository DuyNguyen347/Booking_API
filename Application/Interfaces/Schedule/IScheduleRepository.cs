using Application.Interfaces.Repositories;
using Domain.Constants.Enum;
namespace Application.Interfaces.Schedule
{
    public interface IScheduleRepository : IRepositoryAsync<Domain.Entities.Schedule.Schedule, long>
    {
        string? GetFilmName(long scheduleId);
        string? GetCinemaName(long scheduleId);
        bool IsBookableSchedule(long scheduleId);
        decimal? GetOccupancyRate(long scheduleId);
        IEnumerable<Domain.Entities.Schedule.Schedule> GetCurrPrdScheduleByTimeOption(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0);
        IEnumerable<Domain.Entities.Schedule.Schedule> GetPrevPrdScheduleByTimeOption(StatisticsTimeOption TimeOption, DateTime? FromTime, DateTime? ToTime, long CinemaId = 0);
    }
}
