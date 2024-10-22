using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Cinema
{
    public interface ICinemaRepository : IRepositoryAsync<Domain.Entities.Cinema.Cinema, long>
    {
    }
}
