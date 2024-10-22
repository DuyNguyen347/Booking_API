using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Queries.GetOverview
{
    public class GetOverviewResponse
    {
        public decimal? CurrPrdTotalRevenue { get; set; }
        public int CurrPrdTotalBookings { get; set; }
        public int CurrPrdTotalTickets { get; set; }
        public decimal? CurrPrdOccupancyRate { get; set; }
        public int CurrPrdSchedules { get; set; }

        public decimal? PrevPrdTotalRevenue {  set; get; }
        public int PrevPrdTotalBookings { get; set; }
        public int PrevPrdTotalTickets { get; set; }
        public decimal? PrevPrdOccupancyRate { get; set; }
        public int PrevPrdSchedules { get; set; }
    }
}
