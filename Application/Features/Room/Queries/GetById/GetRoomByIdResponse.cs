using Domain.Constants.Enum;
using Domain.Entities.Seat;

namespace Application.Features.Room.Queries.GetById
{
    public class GetRoomByIdResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int NumberSeat { get; set; }
        public long  CinemaId { get; set; }
        public int NumberRow { get; set; }
        public int NumberColumn { get; set; }
        public SeatStatus Status { get; set; }
        public string? StatusString { get; set; }   
        public List<Domain.Entities.Seat.Seat>? ListSeats { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
