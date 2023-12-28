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
        public decimal? Revenue { get; set; }
        public int NumberOfBookings { get; set; }
        public int NumberOfTickets { get; set; }
        public int NumberOfSchedules { get; set; }
        public decimal? OccupancyRate {  get; set; }
    }
}
