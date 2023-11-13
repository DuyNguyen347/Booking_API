using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetAll.GetAllSchedule
{
    public class GetAllScheduleResponse
    {
        public string? City {  get; set; }
        public string? Name { get; set; }
        public Dictionary<long, GetAllScheduleFilmResponse> Films {  get; set; }
    }
    public class GetAllScheduleFilmResponse
    {
        public string? Name { get; set; }
        public int Duration { get; set; }
        public int LimitAge { get; set; }
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public string? Trailer {  get; set; }
        public string? Image {  get; set; }
        public List<GetAllScheduleScheduleResponse> Schedules {  get; set; }
    }
    public class GetAllScheduleScheduleResponse
    {
        public long Id { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long RoomId { get; set; }
        public int Price { get; set; }
    }
}
