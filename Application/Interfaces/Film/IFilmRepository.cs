using Application.Interfaces.Repositories;

namespace Application.Interfaces.Film
{
    public interface IFilmRepository : IRepositoryAsync<Domain.Entities.Films.Film, long>
    {
        string GetCategory(long filmId);
        string GetImage(long filmId);
    }
}
