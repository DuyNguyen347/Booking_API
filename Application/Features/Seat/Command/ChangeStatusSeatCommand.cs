using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using Application.Interfaces.Seat;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Seat.Command
{
    public class ChangeStatusSeatCommand : IRequest<Result<ChangeStatusSeatCommand>>
    {
        public List<int>? ListNumberSeat { get; set; }
        public long RoomId { get; set; }
        public SeatStatus Status { get; set; } 
    }
    internal class ChangeStatusSeatCommandHandle : IRequestHandler<ChangeStatusSeatCommand, Result<ChangeStatusSeatCommand>>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        
        public ChangeStatusSeatCommandHandle(IRoomRepository roomRepository, ISeatRepository seatRepository, IUnitOfWork<long> unitOfWork)
        {
            _roomRepository = roomRepository;
            _seatRepository = seatRepository;
            _unitOfWork = unitOfWork;
        } 

        public async Task<Result<ChangeStatusSeatCommand>> Handle(ChangeStatusSeatCommand request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.FindAsync(x => x.Id == request.RoomId && !x.IsDeleted);
            if (room == null)
            {
                return await Result<ChangeStatusSeatCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            List<Domain.Entities.Seat.Seat> listSeats = await _seatRepository.Entities.Where(x => request.ListNumberSeat.Contains(x.NumberSeat) && x.RoomId == request.RoomId).ToListAsync();

            //foreach (var seat in listSeats)
            //{
            //    seat.Status = request.Status;
            //}

            await _seatRepository.UpdateRangeAsync(listSeats);
            await _unitOfWork.Commit(cancellationToken);
            
            return await Result<ChangeStatusSeatCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
