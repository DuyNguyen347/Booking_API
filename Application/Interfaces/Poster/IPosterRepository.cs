using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Poster
{
    public interface IPosterRepository : IRepositoryAsync<Domain.Entities.Poster.Poster, long>
    {
    }
}
