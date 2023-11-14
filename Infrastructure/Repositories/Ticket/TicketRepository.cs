using Application.Interfaces.Ticket;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Ticket
{
    public class TicketRepository: RepositoryAsync<Domain.Entities.Ticket.Ticket, long>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
