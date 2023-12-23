namespace Application.Features.Statistics.Queries.GetCinemaStatistic
{
    public class GetCinemaStatisticResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Hotline { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? Address { get; set; }
        public decimal? Revenue {  get; set; }
        public int NumberOfTickets {  get; set; }
    }
}
