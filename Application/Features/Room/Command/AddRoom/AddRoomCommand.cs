using Application.Interfaces.Cinema;
using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using AutoMapper;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Room.Command.AddRoom
{
    public class AddRoomCommand : IRequest<Result<AddRoomCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public int NumberSeat{ get; set; } = default!;
        public SeatStatus Status { get; set; } = default!;
        public long CinemaId { get; set; }
    }

    internal class AddRoomCommandHandler : IRequestHandler<AddRoomCommand, Result<AddRoomCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICinemaRepository _cinemaRepository;


        public AddRoomCommandHandler(IMapper mapper, IRoomRepository roomRepository, IUnitOfWork<long> unitOfWork, ICinemaRepository cinemaRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
            _cinemaRepository = cinemaRepository;
        }

        public async Task<Result<AddRoomCommand>> Handle(AddRoomCommand request, CancellationToken cancellationToken)
        {
            var existCinema = await _cinemaRepository.FindAsync(x => x.Id == request.CinemaId && !x.IsDeleted);
            if (existCinema == null) return await Result<AddRoomCommand>.FailAsync("NOT_FOUND_CINEMA");
            var addRoom = _mapper.Map<Domain.Entities.Room.Room>(request);
            await _roomRepository.AddAsync(addRoom);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addRoom.Id;
            return await Result<AddRoomCommand>.SuccessAsync(request);
        }
    }
}
