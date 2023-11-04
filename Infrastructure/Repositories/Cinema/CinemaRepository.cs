using Application.Interfaces.Category;
using Application.Interfaces.Cinema;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Cinema
{
    public class CinemaRepository : RepositoryAsync<Domain.Entities.Cinema.Cinema, long>, ICinemaRepository
    {
        public CinemaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
