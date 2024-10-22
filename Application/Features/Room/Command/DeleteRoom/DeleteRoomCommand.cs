using Application.Interfaces.Room;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Seat;
using Domain.Entities.Seat;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Room.Command.DeleteRoom
{
    public class DeleteRoomCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }

    }

    internal class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly ISeatRepository _seatRepository;

        public DeleteRoomCommandHandler(
            IRoomRepository RoomRepository,
            IUnitOfWork<long> unitOfWork, ISeatRepository seatRepository)
        {
            _RoomRepository = RoomRepository;
            _unitOfWork = unitOfWork;
            _seatRepository = seatRepository;
        }

        public async Task<Result<long>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var deleteRoom = await _RoomRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deleteRoom == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            // remove seat in room
            List<Domain.Entities.Seat.Seat> listSeatInRoom = await _seatRepository.Entities.Where(x => x.RoomId == deleteRoom.Id).ToListAsync();
            await _seatRepository.RemoveRangeAsync(listSeatInRoom);

            // delete room
            await _RoomRepository.DeleteAsync(deleteRoom);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync("Delete success");
        }
    }
}
