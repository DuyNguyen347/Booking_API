using Application.Features.SeatReservation.Command.AddCommand;
using Application.Interfaces;
using Application.Interfaces.Customer;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SeatReservation.Command.UnlockCommand
{
    public class UnlockSeatReservationCommand: IRequest<Result<UnlockSeatReservationCommand>>
    {
        public long ScheduleId { get; set; }
        public List<int> NumberSeats { get; set; }
    }
    public class UnlockSeatReservationCommandHandler : IRequestHandler<UnlockSeatReservationCommand, Result<UnlockSeatReservationCommand>>
    {
        private readonly ISeatReservationService _seatReservationService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;
        public UnlockSeatReservationCommandHandler(
            ISeatReservationService seatReservationService,
            ICustomerRepository customerRepository,
            IScheduleRepository scheduleRepository,
            ICurrentUserService currentUserService,
            UserManager<AppUser> userManager)
        {
            _seatReservationService = seatReservationService;
            _customerRepository = customerRepository;
            _scheduleRepository = scheduleRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<Result<UnlockSeatReservationCommand>> Handle(UnlockSeatReservationCommand request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
            if (existSchedule == null) return await Result<UnlockSeatReservationCommand>.FailAsync("NOT_FOUND_SCHEDULE");
            _seatReservationService.UnlockSeats(userId, request.ScheduleId, request.NumberSeats);
            return Result<UnlockSeatReservationCommand>.Success(request, "UNLOCKED_SUCCESSFULLY");
        }
    }
}
