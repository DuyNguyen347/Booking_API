using Domain.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Room.Queries.GetAll
{
    public class GetAllRoomResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int NumberSeat { get; set; }
        public SeatStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
