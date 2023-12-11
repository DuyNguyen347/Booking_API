namespace Application.Features.Poster.Queries.GetAll
{
    public class GetAllPosterReponse
    {
        public long Id { get; set; }
        public string? PathImage { get; set; }
        public string? LinkUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
