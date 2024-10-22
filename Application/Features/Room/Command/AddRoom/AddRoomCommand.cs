using Application.Exceptions;
using Application.Interfaces.Cinema;
using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using Application.Interfaces.Seat;
using AutoMapper;
using Domain.Constants.Enum;
using Domain.Entities.Seat;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Room.Command.AddRoom
{
    public class AddRoomCommand : IRequest<Result<AddRoomCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public int NumberSeat{ get; set; } = default!;
        public int NumberRow { get; set; }
        public int NumberColumn { get; set; }
        public SeatStatus Status { get; set; } = default!;
        public long CinemaId { get; set; }
    }

    internal class AddRoomCommandHandler : IRequestHandler<AddRoomCommand, Result<AddRoomCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly ISeatRepository _seatReposity;


        public AddRoomCommandHandler(IMapper mapper, IRoomRepository roomRepository, IUnitOfWork<long> unitOfWork, ICinemaRepository cinemaRepository, ISeatRepository seatReposity)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
            _cinemaRepository = cinemaRepository;
            _seatReposity = seatReposity;
        }

        public async Task<Result<AddRoomCommand>> Handle(AddRoomCommand request, CancellationToken cancellationToken)
        {
            if(request.NumberRow <= 0 || request.NumberRow >= 27 || request.NumberColumn <= 0) return await Result<AddRoomCommand>.FailAsync("INVALID_NUMBER");
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existCinema = await _cinemaRepository.FindAsync(x => x.Id == request.CinemaId && !x.IsDeleted);
                if (existCinema == null) return await Result<AddRoomCommand>.FailAsync("NOT_FOUND_CINEMA");
                var addRoom = _mapper.Map<Domain.Entities.Room.Room>(request);
                await _roomRepository.AddAsync(addRoom);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = addRoom.Id;

                // add seat to room
                List<Domain.Entities.Seat.Seat> listSeat = new List<Domain.Entities.Seat.Seat>();

                for(int i = 1; i <= request.NumberRow; i++)
                {
                    for(int j = 1;j <= request.NumberColumn; j++)
                    {
                        listSeat.Add(new Domain.Entities.Seat.Seat
                        {
                            NumberSeat = (i -1) * request.NumberColumn + j,
                            RoomId = addRoom.Id,
                            //Status = SeatStatus.Available,
                            SeatCode = ((char)('A' + i - 1)).ToString() + j.ToString()
                        });
                    }
                }
                await _seatReposity.AddRangeAsync(listSeat);
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                
                return await Result<AddRoomCommand>.SuccessAsync(request);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
