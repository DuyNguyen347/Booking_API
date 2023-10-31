using Application.Interfaces.Schedule;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Schedule
{
    public class ScheduleRepository : RepositoryAsync<Domain.Entities.Schedule.Schedule, long>, IScheduleRepository
    {
        public ScheduleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
