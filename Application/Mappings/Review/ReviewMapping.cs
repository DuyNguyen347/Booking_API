using Application.Features.Review.Command.Vote;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Review
{
    public class ReviewMapping: Profile
    {
        public ReviewMapping()
        {
            CreateMap<VoteCommand, Domain.Entities.Review.Review>().ReverseMap();
        }
    }
}
