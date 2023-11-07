using Application.Features.Schedule.Command.AddSchedule;
using Application.Features.Schedule.Command.EditSchedule;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Schedule
{
    public class ScheduleMapping : Profile
    {
        public ScheduleMapping()
        {
            CreateMap<Domain.Entities.Schedule.Schedule, AddScheduleCommand>().ReverseMap();
            CreateMap<Domain.Entities.Schedule.Schedule, EditScheduleCommand>().ReverseMap();
        }
    }
}
