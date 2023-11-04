using Application.Features.Cinema.Command.AddCinema;
using Application.Features.Cinema.Command.EditCinema;
using AutoMapper;

namespace Application.Mappings.Cinema
{
    public class CinemaMappings : Profile
    {
        public CinemaMappings()
        {
            //CreateMap<Domain.Entities.Category.Category, Domain.Entities.Category.Category>().ReverseMap();
            CreateMap<AddCinemaCommand, Domain.Entities.Cinema.Cinema>().ReverseMap();
            CreateMap<EditCinemaCommand, Domain.Entities.Cinema.Cinema>().ReverseMap();
        }
    }
}
