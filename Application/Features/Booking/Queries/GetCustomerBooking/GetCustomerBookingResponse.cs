using Application.Dtos.Responses.ServiceImage;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingResponse
    {
        public long Id { get; set; }
        public DateTime? BookingDate { get; set; }
        public decimal? TotalPrice {  get; set; }
    }
}
