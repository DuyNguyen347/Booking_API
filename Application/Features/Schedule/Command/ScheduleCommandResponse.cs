using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Command
{
    public class ScheduleCommandResponse
    {
        public long Id { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public long FilmId { get; set; }
        public long RoomId { get; set; }
        public int Price { get; set; }
    }
}
