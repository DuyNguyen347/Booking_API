using Application.Interfaces.Booking;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Domain.Constants;
using Application.Interfaces.Customer;
using Application.Interfaces;

namespace Application.Features.Booking.Command.UpdateStatusBooking
{
    public class UpdateStatusBookingCommand : IRequest<Result<UpdateStatusBookingCommand>>
    {
        public long Id { get; set; }
        public int BookingStatus { get; set; }
    }

    internal class EditBookingCommandHandler : IRequestHandler<UpdateStatusBookingCommand, Result<UpdateStatusBookingCommand>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;

        public EditBookingCommandHandler(
            IBookingRepository bookingRepository,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UpdateStatusBookingCommand>> Handle(UpdateStatusBookingCommand request, CancellationToken cancellationToken)
        {
            var ExistBooking = await _bookingRepository.FindAsync(x => !x.IsDeleted && x.Id == request.Id);
            if (ExistBooking == null) return await Result<UpdateStatusBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_BOOKING);

            if (!_enumService.CheckEnumExistsById(request.BookingStatus, StaticVariable.BOOKING_STATUS_ENUM)) return await Result<UpdateStatusBookingCommand>.FailAsync(StaticVariable.STATUS_NOT_EXIST);

            ExistBooking.Status = request.BookingStatus;
            await _bookingRepository.UpdateAsync(ExistBooking);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<UpdateStatusBookingCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
