using Application.Features.Ticket.Command;
using AutoMapper;

namespace Application.Mappings.Ticket
{
    public class TicketMapping : Profile
    {
        public TicketMapping()
        {
            CreateMap<Domain.Entities.Ticket.Ticket, AddTicketCommand>().ReverseMap();
        }  

    }
}
