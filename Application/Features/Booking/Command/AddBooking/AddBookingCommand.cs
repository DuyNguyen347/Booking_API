using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Schedule;
using Application.Interfaces.Seat;
using Application.Interfaces.Service;
using Application.Interfaces.Ticket;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.AddBooking
{
    //public class AddBookingCommand : IRequest<Result<AddBookingCommand>>
    //{
    //    public long Id { get; set; }
    //    public long CustomerId { get; set; }
    //    public long ScheduleId { get; set; }    
    //    public long RoomId {  get; set; }
    //    public List<int>? ListNumberSeat {  get; set; }
    //}

    //internal class AddBookingCommandHandler : IRequestHandler<AddBookingCommand, Result<AddBookingCommand>>
    //{
    //    private readonly IMapper _mapper;
    //    private readonly IBookingRepository _bookingRepository;
    //    private readonly IScheduleRepository _scheduleRepository;
    //    private readonly ITicketRepository _ticketRepository;
    //    private readonly ISeatRepository _seatRepository;
    //    private readonly IEnumService _enumService;
    //    private readonly IUnitOfWork<long> _unitOfWork;
    //    private readonly ICustomerRepository _customerRepository;
    //    private readonly ICurrentUserService _currentUserService;
    //    private readonly UserManager<AppUser> _userManager;

    //    public AddBookingCommandHandler(
    //        IMapper mapper, 
    //        IBookingRepository bookingRepository,
    //        IScheduleRepository scheduleRepository,
    //        ITicketRepository ticketRepository,
    //        ISeatRepository seatRepository,
    //        IEnumService enumService, 
    //        IUnitOfWork<long> unitOfWork, 
    //        ICustomerRepository customerRepository, 
    //        ICurrentUserService currentUserService, 
    //        UserManager<AppUser> userManager)
    //    {
    //        _mapper = mapper;
    //        _bookingRepository = bookingRepository;
    //        _scheduleRepository = scheduleRepository;
    //        _ticketRepository = ticketRepository;
    //        _seatRepository = seatRepository;
    //        _enumService = enumService;
    //        _unitOfWork = unitOfWork;
    //        _customerRepository = customerRepository;
    //        _currentUserService = currentUserService;
    //        _userManager = userManager;
    //    }

    //    public async Task<Result<AddBookingCommand>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
    //    {
    //        if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
    //        {
    //            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

    //            if (userId != request.CustomerId)
    //                return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
    //        }

    //        //open transaction
    //        var transaction = await _unitOfWork.BeginTransactionAsync();
    //        try
    //        {
    //            var ExistCustomer = await _customerRepository.FindAsync(x => x.Id == request.CustomerId && !x.IsDeleted);
    //            if (ExistCustomer == null) return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

    //            var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
    //            if (existSchedule == null) return await Result<AddBookingCommand>.FailAsync("NOT_FOUND_SCHEDULE");

    //            var booking = new Domain.Entities.Booking.Booking()
    //            {
    //                CustomerId = request.CustomerId,
    //                ScheduleId = request.ScheduleId
    //            };
    //            booking.Status = _enumService.GetEnumIdByValue(StaticVariable.WAITING, StaticVariable.BOOKING_STATUS_ENUM);
    //            await _bookingRepository.AddAsync(booking);
    //            await _unitOfWork.Commit(cancellationToken);
    //            request.Id = booking.Id;

    //            List<Domain.Entities.Seat.Seat> listSeats = await _seatRepository.Entities.Where(x => request.ListNumberSeat.Contains(x.NumberSeat) && x.RoomId == request.RoomId).ToListAsync();

    //            foreach (var seat in listSeats)
    //                if (seat.Status != Domain.Constants.Enum.SeatStatus.Available) 
    //                    return await Result<AddBookingCommand>.FailAsync("SEAT_NOT_AVAILABLE");
    //            foreach (var seat in listSeats)
    //            {
    //                seat.Status = Domain.Constants.Enum.SeatStatus.Disabled;
    //            }
    //            await _unitOfWork.Commit(cancellationToken);
    //            await transaction.CommitAsync(cancellationToken);
    //            return await Result<AddBookingCommand>.SuccessAsync(request);
    //        }
    //        catch (Exception ex)
    //        {
    //            await transaction.RollbackAsync(cancellationToken);
    //            throw new ApiException(ex.Message);
    //        }
    //        finally
    //        {
    //            await transaction.DisposeAsync();
    //        }
    //    }
    //}
}