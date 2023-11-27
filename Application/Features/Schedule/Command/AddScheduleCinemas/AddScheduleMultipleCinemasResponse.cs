using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Command.AddScheduleCinemas
{
    public class AddScheduleMultipleCinemasResponse
    {
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public List<CinemaSchedule> CinemaSchedules { get; set; } = new List<CinemaSchedule>();
    }
    public class CinemaSchedule
    {
        public long Id { get; set; }
        public long CinemaId { get; set; }
        public string? CinemaName { get; set; }
        public long RoomId { get; set; }
    }
}
