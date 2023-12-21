namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingResponse
    {
        public long Id { get; set; }
        public string? BookingRefId { get; set; }

        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? BookingDate { get; set; }
        public string? FilmName { get; set; }
        public string? CinemaName { get; set; }
        public DateTime CreatedOn {  get; set; }
        public DateTime? LastModifiedOn {  get; set; }

    }
}
