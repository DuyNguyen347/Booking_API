using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ScheduleSeat
{
    public interface IScheduleSeatRepository: IRepositoryAsync<Domain.Entities.ScheduleSeat.ScheduleSeat, long>
    {
    }
}
