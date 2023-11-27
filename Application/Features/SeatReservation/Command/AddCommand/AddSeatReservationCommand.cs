using Application.Features.Schedule.Command;
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

namespace Application.Features.SeatReservation.Command.AddCommand
{
    public class AddSeatReservationCommand : IRequest<Result<AddSeatReservationCommand>>
    {
        public long CustomerId { get; set; }
        public long ScheduleId { get; set; }
        public List<int> NumberSeats { get; set; }
    }
    public class AddSeatReservationCommandHandler : IRequestHandler<AddSeatReservationCommand, Result<AddSeatReservationCommand>>
    {
        private readonly ISeatReservationService _seatReservationService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        public AddSeatReservationCommandHandler(
            ISeatReservationService seatReservationService,
            ICustomerRepository customerRepository,
            IScheduleRepository scheduleRepository)
        {
            _seatReservationService = seatReservationService;
            _customerRepository = customerRepository;
            _scheduleRepository = scheduleRepository;
        }
        public async Task<Result<AddSeatReservationCommand>> Handle(AddSeatReservationCommand request, CancellationToken cancellationToken)
        {
            var existCustomer = await _customerRepository.FindAsync(x => x.Id == request.CustomerId && !x.IsDeleted);
            if (existCustomer == null) return await Result<AddSeatReservationCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);
            var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
            if (existSchedule == null) return await Result<AddSeatReservationCommand>.FailAsync("NOT_FOUND_SCHEDULE");
            bool IsSuccess = _seatReservationService.LockSeats(request.CustomerId, request.ScheduleId, request.NumberSeats);
            if (!IsSuccess)
                return Result<AddSeatReservationCommand>.Fail("SEATS_UNAVAILABLE_TEMPORARILY");
            return Result<AddSeatReservationCommand>.Success(request, "RESERVED_SUCCESSFULLY");
        }
    }
}
