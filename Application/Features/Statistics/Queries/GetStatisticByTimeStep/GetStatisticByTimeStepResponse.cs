namespace Application.Features.Statistics.Queries.GetStatisticByTimeStep
{
    public class GetStatisticByTimeStepResponse
    {
        public string? Label { get; set; }
        public decimal? TotalRevenue { get; set; }
        public int NumberOfBookings { get; set; }
        public int NumberOfTickets {  get; set; }
    }
}
