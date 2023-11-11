using Application.Features.Room.Queries.GetById;
using Application.Interfaces.Category;
using Application.Interfaces.Room;
using Application.Interfaces;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Seat;
using Microsoft.EntityFrameworkCore;
using Domain.Constants.Enum;

namespace Application.Features.Room.Queries.GetById
{
    public class GetRoomByIdQuery : IRequest<Result<GetRoomByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<GetRoomByIdResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly ISeatRepository _seatRepository;

        public GetRoomByIdQueryHandler(IRoomRepository RoomRepository,ISeatRepository seatRepository)
        {
            _RoomRepository = RoomRepository;
            _seatRepository = seatRepository;
        }

        public async Task<Result<GetRoomByIdResponse>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var room = _RoomRepository.Entities.Where(x => x.Id == request.Id && !x.IsDeleted).Select(x => new GetRoomByIdResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Status = x.Status,
                CinemaId = x.CinemaId,
                NumberSeat = x.NumberSeat,
                NumberRow = x.NumberRow,
                NumberColumn = x.NumberColumn,
                CreatedOn = x.CreatedOn,
                LastModifiedOn = x.LastModifiedOn
            }).FirstOrDefault();
            if (room == null) return await Result<GetRoomByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            room.ListSeats = await _seatRepository.Entities.Where(x => x.RoomId == room.Id).ToListAsync();
            return await Result<GetRoomByIdResponse>.SuccessAsync(room);
        }

    }
}
