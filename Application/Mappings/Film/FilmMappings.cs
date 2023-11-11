using Application.Features.Film.Command.AddFilm;
using Application.Features.Film.Command.EditFilm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Film
{
    public class FilmMappings : Profile
    {
        public FilmMappings()
        {
            CreateMap<Domain.Entities.Films.Film, AddFilmCommand>().ReverseMap();
            CreateMap<Domain.Entities.Films.Film, EditFilmCommand>().ReverseMap();
        }
    }
}
