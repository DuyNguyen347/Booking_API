using Application.Features.Poster.Command.AddPoster;
using AutoMapper;

namespace Application.Mappings.Poster
{
    public class PosterMappings : Profile
    {
        public PosterMappings()
        {
            CreateMap<AddPosterCommand, Domain.Entities.Poster.Poster>().ReverseMap();
        }
    }
}
