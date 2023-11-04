using Application.Interfaces.Room;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DeleteRoomCommandHandler(
            IRoomRepository RoomRepository,
            IUnitOfWork<long> unitOfWork)
        {
            _RoomRepository = RoomRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var deleteRoom = await _RoomRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deleteRoom == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            await _RoomRepository.DeleteAsync(deleteRoom);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync("Delete success");
        }
    }
}
