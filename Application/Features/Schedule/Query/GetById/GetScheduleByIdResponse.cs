﻿using Domain.Constants.Enum;
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
        public DateTime EndTime { get; set; }
        public long FilmId { get; set; }
        public long CinemaId {  get; set; }
        public long RoomId {  get; set; }
        public int Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn {  get; set; }
        public List<GetScheduleByIdSeatResponse>? scheduleSeats { get; set; }
    }
    public class GetScheduleByIdSeatResponse
    {
        public long Id { get; set; }
        public int NumberSeat {  get; set; }
        public string? SeatCode {  get; set; }
        public SeatStatus Status { get; set; }
    }
}
