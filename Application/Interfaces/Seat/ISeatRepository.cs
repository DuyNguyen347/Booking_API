using Application.Interfaces.Repositories;

namespace Application.Interfaces.Seat
{
    public interface ISeatRepository : IRepositoryAsync<Domain.Entities.Seat.Seat, long>
    {
    }
}
