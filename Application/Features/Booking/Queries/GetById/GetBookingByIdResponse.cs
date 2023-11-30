using Domain.Constants.Enum;
using System.Security.Cryptography.Pkcs;

namespace Application.Features.Booking.Queries.GetById
{
    public class GetBookingByIdResponse
    {
        public long Id { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? BookingCurrency {  get; set; }
        public string? BookingLanguage {  get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public string? CinemaName {  get; set; }
        public string? FilmName {  get; set; }
        public string? RoomName { get; set; }
        public string? Image {  get; set; }
        public List<TicketBookingResponse> Tickets { get; set; } = new List<TicketBookingResponse>();
        public string? QRCode { get; set; }
    }

    public class TicketBookingResponse
    {
        public long Id { get; set; }
        public int NumberSeat {  get; set; }
        public string? SeatCode {  get; set; }
        public TypeTicket TypeTicket { get; set; }
        public int Price { get; set; }
    }
}