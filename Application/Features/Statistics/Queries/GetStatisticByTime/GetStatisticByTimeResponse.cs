namespace Application.Features.Statistics.Queries.GetStatisticByTime
{
    public class GetStatisticByTimeResponse
    {
        public string? Label { get; set; }
        public decimal? Revenue { get; set; }
        public int NumberOfTickets {  get; set; }
    }
}
