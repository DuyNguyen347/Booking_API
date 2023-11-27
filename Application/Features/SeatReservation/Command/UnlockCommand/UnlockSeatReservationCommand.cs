using Application.Features.SeatReservation.Command.AddCommand;
using Application.Interfaces.Customer;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SeatReservation.Command.UnlockCommand
{
    public class UnlockSeatReservationCommand: IRequest<Result<UnlockSeatReservationCommand>>
    {
        public long CustomerId { get; set; }
        public long ScheduleId { get; set; }
        public List<int> NumberSeats { get; set; }
    }
    public class UnlockSeatReservationCommandHandler : IRequestHandler<UnlockSeatReservationCommand, Result<UnlockSeatReservationCommand>>
    {
        private readonly ISeatReservationService _seatReservationService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        public UnlockSeatReservationCommandHandler(
            ISeatReservationService seatReservationService,
            ICustomerRepository customerRepository,
            IScheduleRepository scheduleRepository)
        {
            _seatReservationService = seatReservationService;
            _customerRepository = customerRepository;
            _scheduleRepository = scheduleRepository;
        }
        public async Task<Result<UnlockSeatReservationCommand>> Handle(UnlockSeatReservationCommand request, CancellationToken cancellationToken)
        {
            var existCustomer = await _customerRepository.FindAsync(x => x.Id == request.CustomerId && !x.IsDeleted);
            if (existCustomer == null) return await Result<UnlockSeatReservationCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);
            var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
            if (existSchedule == null) return await Result<UnlockSeatReservationCommand>.FailAsync("NOT_FOUND_SCHEDULE");
            _seatReservationService.UnlockSeats(request.CustomerId, request.ScheduleId, request.NumberSeats);
            return Result<UnlockSeatReservationCommand>.Success(request, "UNLOCKED_SUCCESSFULLY");
        }
    }
}
