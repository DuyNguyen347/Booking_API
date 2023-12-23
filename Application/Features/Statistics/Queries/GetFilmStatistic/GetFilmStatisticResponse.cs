namespace Application.Features.Statistics.Queries.GetFilmStatistic
{
    public class GetFilmStatisticResponse
    {
        public long Id { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public int Duration { get; set; }
        public string? Category {  get; set; }
        public int NumberOfVotes { get; set; }
        public decimal? Score { get; set; }
        public decimal? Revenue { get; set; }
        public int NumberOfTickets {  get; set; }
    }
}
