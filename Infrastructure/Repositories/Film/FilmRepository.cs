using Application.Interfaces.Film;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Film
{
    public class FilmRepository : RepositoryAsync<Domain.Entities.Films.Film, long>, IFilmRepository
    {
        public FilmRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
