using Application.Interfaces.Repositories;

namespace Application.Interfaces.Room
{
    public interface IRoomRepository : IRepositoryAsync<Domain.Entities.Room.Room, long>
    {
    }
}
