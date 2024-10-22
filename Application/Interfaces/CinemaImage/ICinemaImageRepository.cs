using Application.Interfaces.Repositories;

namespace Application.Interfaces.CinemaImage
{
    public interface ICinemaImageRepository : IRepositoryAsync<Domain.Entities.CinemaImage.CinemaImage, long>
    {
    }
}
