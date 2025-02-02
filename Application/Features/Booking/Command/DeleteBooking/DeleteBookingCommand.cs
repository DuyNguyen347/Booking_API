using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Ticket;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.DeleteBooking
{
    public class DeleteBookingCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }

    internal class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEnumService _enumService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public DeleteBookingCommandHandler(
            IUnitOfWork<long> unitOfWork,
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEnumService enumService,
            ITicketRepository ticketRepository, 
            ICurrentUserService currentUserService, 
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _customerRepository = customerRepository;
            _enumService = enumService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<long>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var booking = await _bookingRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                if (booking == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
                if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
                {
                    long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                    if (userId != booking.CustomerId)
                        return await Result<long>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
                }

                if (booking.Status != _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                {
                    var tickets = await (from ticket in _ticketRepository.Entities
                                         where !ticket.IsDeleted && ticket.BookingId == booking.Id
                                         select ticket).ToListAsync();
                    await _ticketRepository.DeleteRange(tickets);
                    await _bookingRepository.DeleteAsync(booking);
                    await _unitOfWork.Commit(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return await Result<long>.SuccessAsync($"BOOKING_{request.Id}_DELETED_SUCCESSFULLY");
                }
                return await Result<long>.FailAsync("PURCHASED_BOOKING");
            }
            catch (Exception ex)
            {
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