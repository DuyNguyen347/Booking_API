using Application.Interfaces.Room;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Domain.Constants.Enum;
using System.Security.AccessControl;
using Application.Interfaces.Seat;
using Domain.Entities.Seat;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Room.Command.EditRoom
{
    public class EditRoomCommand : IRequest<Result<EditRoomCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public int NumberSeat { get; set; }
        public int NumberRow { get; set; }
        public int NumberColumn { get; set; }
        public SeatStatus Status { get; set; }
        public long CinemaId { get; set; }
    }

    internal class EditRoomCommandHandler : IRequestHandler<EditRoomCommand, Result<EditRoomCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly ISeatRepository _seatRepository;

        public EditRoomCommandHandler(IMapper mapper, IRoomRepository RoomRepository, IUnitOfWork<long> unitOfWork, ISeatRepository seatRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
            _seatRepository = seatRepository;
        }

        public async Task<Result<EditRoomCommand>> Handle(EditRoomCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditRoomCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            var editRoom = await _RoomRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editRoom == null) return await Result<EditRoomCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            if(request.NumberColumn != editRoom.NumberColumn || request.NumberRow != editRoom.NumberRow)
            {
                List<Domain.Entities.Seat.Seat> listSeatCurrent = await _seatRepository.Entities.Where(x => x.RoomId == editRoom.Id).ToListAsync();
                await _seatRepository.RemoveRangeAsync(listSeatCurrent);

                List<Domain.Entities.Seat.Seat> listSeat = new List<Domain.Entities.Seat.Seat>();

                for (int i = 1; i <= request.NumberRow; i++)
                {
                    for (int j = 1; j <= request.NumberColumn; j++)
                    {
                        listSeat.Add(new Domain.Entities.Seat.Seat
                        {
                            NumberSeat = (i - 1) * request.NumberColumn + j,
                            RoomId = editRoom.Id,
                            //Status = SeatStatus.Available,
                            SeatCode = ((char)('A' + i - 1)).ToString() + j.ToString()
                        });
                    }
                }

                await _seatRepository.AddRangeAsync(listSeat);
            }
            _mapper.Map(request, editRoom);
            await _RoomRepository.UpdateAsync(editRoom);

            await _unitOfWork.Commit(cancellationToken);
            request.Id = editRoom.Id;
            return await Result<EditRoomCommand>.SuccessAsync(request);
        }
    }
}
