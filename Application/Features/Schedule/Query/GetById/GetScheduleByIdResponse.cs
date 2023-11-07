using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetById
{
    public class GetScheduleByIdResponse
    {
        public long Id { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime {  get; set; }
        public string? Film { get; set; }
        public string? Cinema {  get; set; }
        public string? Room {  get; set; }
        public int Price { get; set; }
}
}
