using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Queries.GetDaytimeRangesStatistic
{
    public class GetDaytimeRangesStatisticResponse
    {
        public int StartHour {  get; set; }
        public int EndHour { get; set; }
        public decimal? FcsBookingRevenue {  get; set; }
        public int FcsBookingNumberOfBookings { get; set; }
        public int FcsBookingNumberOfTickets {  get; set; }
        public int FcsBookingNumberOfSchedules {  get; set; }
        public decimal? FcsScheduleRevenue { get; set; }
        public int FcsScheduleNumberOfBookings { get; set; }
        public int FcsScheduleNumberOfTickets { get; set; }
        public int FcsScheduleNumberOfSchedules { get; set; }
    }
}
