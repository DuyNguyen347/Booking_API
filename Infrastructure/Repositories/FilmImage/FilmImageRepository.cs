using Application.Interfaces.BookingDetail;
using Application.Interfaces.FilmImage;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.FilmImage
{
    public class FilmImageRepository : RepositoryAsync<Domain.Entities.FilmImage.FilmImage, long>, IFilmImageRepository
    {
        public FilmImageRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            
        }
    }
}
