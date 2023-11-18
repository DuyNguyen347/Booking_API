using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.Ticket;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : IRequest<Result<List<GetCustomerBookingResponse>>>
    {
        [Required]
        public long CustomerId { get; set; }

        public string? KeyWord { get; set; }

        public int? BookingStatus { get; set; }
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, Result<List<GetCustomerBookingResponse>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IEnumService _enumService;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
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
            _ticketRepository = ticketRepository;
            _enumService = enumService;
            _uploadService = uploadService;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<List<GetCustomerBookingResponse>>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            if (userId != request.CustomerId)
                return await Result<List<GetCustomerBookingResponse>>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);

            if (request.BookingStatus != null && !_enumService.CheckEnumExistsById((int)request.BookingStatus, StaticVariable.BOOKING_STATUS_ENUM))
                return await Result<List<GetCustomerBookingResponse>>.FailAsync(StaticVariable.STATUS_NOT_EXIST);

            var bookings = await _bookingRepository.Entities.Where(_ => !_.IsDeleted && _.CustomerId == request.CustomerId)
                .Select(s => new Domain.Entities.Booking.Booking
                {
                    Id = s.Id,
                    CustomerId = s.CustomerId,
                    LastModifiedOn = s.LastModifiedOn,
                    Status = s.Status,
                }).ToListAsync();
            List<GetCustomerBookingResponse> response = new List<GetCustomerBookingResponse>();
            foreach (var booking in bookings)
            {
                var bookingResponse = new GetCustomerBookingResponse
                {
                    BookingId = booking.Id,
                    BookingStatus = booking.Status,
                    LastModifiedOn = booking.LastModifiedOn,
                };

                bool matchWithRequiredStatus = request.BookingStatus == null ? true : false;

                if (bookingResponse.BookingStatus == request.BookingStatus)
                    matchWithRequiredStatus = true;

                if (matchWithRequiredStatus)
                {
                    var bookingTickets = await _ticketRepository.Entities.Where(_ => !_.IsDeleted && _.BookingId == booking.Id)
                    .Select(s => new Domain.Entities.BookingDetail.BookingDetail
                    {
                        Id = s.Id,
                        BookingId = booking.Id,
                    }).ToListAsync();

                    bool checkServiceName = false;
                    List<BookingDetailResponse> bookingDetailResponses = new List<BookingDetailResponse>();
                }
            }
            return await Result<List<GetCustomerBookingResponse>>.SuccessAsync(response);
        }
    }
}