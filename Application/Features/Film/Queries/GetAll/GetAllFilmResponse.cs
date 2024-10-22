using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Application.Features.Film.Queries.GetAll
{
    public class GetAllFilmResponse
    {
        public long Id { get; set; }
        public bool Enable { get; set; }
        public string? Name { get; set; }
        public string? Actor { get ; set; }
        public string? Director { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public string ? Producer { get; set; }
        public string? Country { get; set; }
        public int LimitAge { get; set; }
        public string? Trailer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? Poster { get; set; }
        public int NumberOfVotes {  get; set; }
        public decimal? Score {  get; set; }
    }
}
