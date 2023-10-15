using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.CategoryFilm
{
    public interface ICategoryFilmRepository : IRepositoryAsync<Domain.Entities.CategoryFilm.CategoryFilm, long>
    {
    }
}
