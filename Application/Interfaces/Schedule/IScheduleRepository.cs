using Application.Interfaces.Repositories;

namespace Application.Interfaces.Schedule
{
    public interface IScheduleRepository : IRepositoryAsync<Domain.Entities.Schedule.Schedule, long>
    {
    }
}
