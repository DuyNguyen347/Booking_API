using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Booking.Command.UpdateUsageStatus
{
    public class UpdateUsageStatusCommand : IRequest<Result<UpdateUsageStatusCommand>>
    {
        public long Id { get; set; }
        public string? UsageStatus { get; set; }
    }

    public class UpdateUsageStatusCommandHandler : IRequestHandler<UpdateUsageStatusCommand, Result<UpdateUsageStatusCommand>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;

        public UpdateUsageStatusCommandHandler(
            IBookingRepository bookingRepository,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UpdateUsageStatusCommand>> Handle(UpdateUsageStatusCommand request, CancellationToken cancellationToken)
        {
            var ExistBooking = await _bookingRepository.FindAsync(x => !x.IsDeleted && x.Id == request.Id);
            if (ExistBooking == null) return await Result<UpdateUsageStatusCommand>.FailAsync(StaticVariable.NOT_FOUND_BOOKING);

            if (string.IsNullOrEmpty(request.UsageStatus) || request.UsageStatus.ToLower() != "used")
                return await Result<UpdateUsageStatusCommand>.FailAsync("Usage status invalid");

            ExistBooking.BookingStatus = request.UsageStatus.ToLower();
            await _bookingRepository.UpdateAsync(ExistBooking);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<UpdateUsageStatusCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
