using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetAll.GetAllSchedule
{
    public class GetAllScheduleResponse
    {
        public long CinemaId { get; set; }
        public string? City {  get; set; }
        public string? Name { get; set; }
        public List<GetAllScheduleFilmResponse> Films {  get; set; } = new List<GetAllScheduleFilmResponse> { };
    }
    public class GetAllScheduleFilmResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Duration { get; set; }
        public int LimitAge { get; set; }
        public string? Actor { get; set; }
        public string? Director { get; set; }
        public string? Producer { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public string? Country { get; set; }
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public string? Trailer {  get; set; }
        public string? Image {  get; set; }
        public decimal? Score {  get; set; }
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
