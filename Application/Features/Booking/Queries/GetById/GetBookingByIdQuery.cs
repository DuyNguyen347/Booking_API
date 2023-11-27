using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Service;
using Application.Interfaces.Ticket;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Queries.GetById
{
    public class GetBookingByIdQuery : IRequest<Result<GetBookingByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public GetBookingByIdQueryHandler(
            IMapper mapper,
            IBookingRepository bookingRepository, 
            ITicketRepository ticketRepository,
            ICustomerRepository customerRepository, 
            ICurrentUserService currentUserService, 
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<Result<GetBookingByIdResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var Booking = await _bookingRepository.Entities
                .Where(booking => booking.Id == request.Id && !booking.IsDeleted)
                .Select(booking => booking).FirstOrDefaultAsync();
            if(Booking == null)
            {
                return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            //if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            //{
            //    long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            //    if (userId != Booking.CustomerId)
            //        return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            //}

            var response = _mapper.Map<GetBookingByIdResponse>(Booking);

            var CustomerBooking = await _customerRepository.Entities
                .Where(_ => _.Id == Booking.CustomerId)
                .Select(s => new Domain.Entities.Customer.Customer
                {
                    Id = s.Id,
                    CustomerName = s.CustomerName,
                    PhoneNumber = s.PhoneNumber,
                }).FirstOrDefaultAsync();
            if(CustomerBooking == null)
            {
                return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            response.CustomerName = CustomerBooking.CustomerName;
            response.PhoneNumber = CustomerBooking.PhoneNumber;
            response.TotalPrice = Booking.RequiredAmount;
            response.FilmId = await (from schedule in _scheduleRepository.Entities
                                     where !schedule.IsDeleted && schedule.Id == Booking.ScheduleId
                                     select schedule.FilmId).FirstOrDefaultAsync();

            List<TicketBookingResponse> bookingTickets = await _ticketRepository.Entities
                .Where(_ => _.BookingId == Booking.Id && !_.IsDeleted)
                .Select(s => new TicketBookingResponse
                {
                    Id = s.Id,
                    TypeTicket = s.Type,
                    Price = s.Price,
                    NumberSeat = s.NumberSeat,
                    SeatCode = s.SeatCode
                }).ToListAsync();
            response.Tickets = bookingTickets;
            return await Result<GetBookingByIdResponse>.SuccessAsync(response);
        }
    }
}
