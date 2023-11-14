using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Ticket
{
    public interface ITicketRepository: IRepositoryAsync<Domain.Entities.Ticket.Ticket, long>
    {
    }
}
