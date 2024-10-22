using Application.Dtos.Requests.Feedback;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.FilmImage
{
    public class FilmImageMappings : Profile
    {
        public FilmImageMappings()
        {
            CreateMap<Domain.Entities.FilmImage.FilmImage, ImageRequest>().ReverseMap();
            //CreateMap<List<Domain.Entities.FilmImage.FilmImage>, List<ImageRequest>>().ReverseMap();
        }
    }
}
