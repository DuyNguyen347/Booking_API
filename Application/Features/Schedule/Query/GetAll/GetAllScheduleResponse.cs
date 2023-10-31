using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetAll
{
    public class GetAllScheduleResponse
    {
        public long Id { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public long FilmId { get; set; }
        public long RoomId { get; set; }
        public int Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
