using Domain.Constants.Enum;
using System.Security.Cryptography.Pkcs;

namespace Application.Features.Booking.Queries.GetById
{
    public class GetBookingByIdResponse
    {
        public long Id { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public long ScheduleId { get; set; }
        public int TotalPrice { get; set; }
        public byte[]? QRCode { get; set; }
        public int? Status { get; set; }
        public List<TicketBookingResponse> Tickets { get; set; } = new List<TicketBookingResponse>();
        public DateTime CreatedOn { get; set; }

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