using Application.Interfaces.Schedule;
using Application.Interfaces.ScheduleSeat;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ScheduleSeat
{
    public class ScheduleSeatRepository: RepositoryAsync<Domain.Entities.ScheduleSeat.ScheduleSeat, long>, IScheduleSeatRepository
    {
        public ScheduleSeatRepository(ApplicationDbContext dbContext) : base(dbContext)
        { 

        }
    }
}
