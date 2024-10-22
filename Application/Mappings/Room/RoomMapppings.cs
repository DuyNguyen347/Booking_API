using Application.Features.Room.Command.AddRoom;
using Application.Features.Room.Command.EditRoom;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Room
{
    public class RoomMapppings : Profile
    {
        public RoomMapppings()
        {
            CreateMap<AddRoomCommand, Domain.Entities.Room.Room>().ReverseMap();
            CreateMap<EditRoomCommand, Domain.Entities.Room.Room>().ReverseMap();
        }
    }
}
