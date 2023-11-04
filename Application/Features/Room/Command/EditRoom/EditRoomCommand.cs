using Application.Interfaces.Room;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Domain.Constants.Enum;

namespace Application.Features.Room.Command.EditRoom
{
    public class EditRoomCommand : IRequest<Result<EditRoomCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public int NumberSeat { get; set; }
        public SeatStatus Status { get; set; }
        public long CinemaId { get; set; }
    }

    internal class EditRoomCommandHandler : IRequestHandler<EditRoomCommand, Result<EditRoomCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IRoomRepository _RoomRepository;

        public EditRoomCommandHandler(IMapper mapper, IRoomRepository RoomRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
        }

        public async Task<Result<EditRoomCommand>> Handle(EditRoomCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditRoomCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            var editRoom = await _RoomRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editRoom == null) return await Result<EditRoomCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            _mapper.Map(request, editRoom);
            await _RoomRepository.UpdateAsync(editRoom);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = editRoom.Id;
            return await Result<EditRoomCommand>.SuccessAsync(request);
        }
    }
}
