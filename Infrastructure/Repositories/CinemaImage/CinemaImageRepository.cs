using Application.Interfaces.CinemaImage;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.CinemaImage
{
    public class CinemaImageRepository : RepositoryAsync<Domain.Entities.CinemaImage.CinemaImage, long>, ICinemaImageRepository
    {
        public CinemaImageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
