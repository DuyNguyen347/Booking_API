using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.FilmImage
{
    public interface IFilmImageRepository : IRepositoryAsync<Domain.Entities.FilmImage.FilmImage, long>
    {
    }
}
