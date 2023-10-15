using Application.Interfaces.BookingDetail;
using Application.Interfaces.CategoryFilm;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.CategoryFilm
{
    public class CategoryFilmRepository : RepositoryAsync<Domain.Entities.CategoryFilm.CategoryFilm, long>, ICategoryFilmRepository
    {
        public CategoryFilmRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
