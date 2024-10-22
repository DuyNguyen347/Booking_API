using Application.Features.Category.Command.AddCategory;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Category
{
    public class CategoryMappings: Profile
    {
        public CategoryMappings ()
        {
            //CreateMap<Domain.Entities.Category.Category, Domain.Entities.Category.Category>().ReverseMap();
            CreateMap<AddCategoryCommand, Domain.Entities.Category.Category>().ReverseMap();
        }
    }
}
