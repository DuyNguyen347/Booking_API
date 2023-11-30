using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.Ticket;
using Application.Parameters;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Entities.Booking;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : RequestParameter, IRequest<Result<List<GetCustomerBookingResponse>>>
    {
        [Required]
        public long CustomerId { get; set; }
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, Result<List<GetCustomerBookingResponse>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public GetCustomerBookingQueryHandler(
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository,
            IMapper mapper, 
            IEnumService enumService, 
            IUploadService uploadService, 
            ICurrentUserService currentUserService, 
            UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<List<GetCustomerBookingResponse>>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            if (userId != request.CustomerId)
                return await Result<List<GetCustomerBookingResponse>>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);

            var bookings = await _bookingRepository.Entities.Where(
                _ => !_.IsDeleted && 
                _.CustomerId == request.CustomerId &&
                _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)) 
                .Select(s => new GetCustomerBookingResponse
                {
                    Id = s.Id,
                    BookingDate = s.BookingDate,
                    TotalPrice = s.RequiredAmount,
                }).ToListAsync();
            return await Result<List<GetCustomerBookingResponse>>.SuccessAsync(bookings);
        }
    }
}