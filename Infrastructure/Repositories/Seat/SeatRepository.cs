using Application.Interfaces.Seat;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Seat
{
    public class SeatRepository : RepositoryAsync<Domain.Entities.Seat.Seat, long>, ISeatRepository
    {
        public SeatRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
