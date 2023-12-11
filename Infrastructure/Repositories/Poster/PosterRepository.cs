using Application.Interfaces.Film;
using Application.Interfaces.Poster;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Poster
{
    public class PosterRepository : RepositoryAsync<Domain.Entities.Poster.Poster, long>, IPosterRepository
    {
        public PosterRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
