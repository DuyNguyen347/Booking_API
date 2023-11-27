namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingResponse
    {
        public long Id { get; set; }

        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }
        public decimal? TotalPrice { get; set; }

        public DateTime? BookingDate { get; set; }

    }
}
