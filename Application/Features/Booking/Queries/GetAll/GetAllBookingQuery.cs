using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Customer;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Parameters;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Entities;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core;

namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingQuery : RequestParameter, IRequest<PaginatedResult<GetAllBookingResponse>>
    {
        public enum BookingMethodOption
        {
            All = 0,
            Online = 1,
            Offline = 2
        }
        public enum PaymentStatusOption
        {
            All = 0,
            Waiting = 1,
            Done = 2
        }
        public long CustomerId { get; set; }
        public BookingMethodOption BookingMethod { get; set; } = BookingMethodOption.All;
        public PaymentStatusOption PaymentStatus { get; set; } = PaymentStatusOption.All;
        public long CinemaId { get; set; }
    }

    internal class GetAllBookingHandler : IRequestHandler<GetAllBookingQuery, PaginatedResult<GetAllBookingResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEnumService _enumService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly UserManager<AppUser> _userManager;

        public GetAllBookingHandler(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEnumService enumService,
            IScheduleRepository scheduleRepository,
            ICinemaRepository cinemaRepository,
            UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _enumService = enumService;
            _scheduleRepository = scheduleRepository;
            _cinemaRepository = cinemaRepository;
            _userManager = userManager;
        }

        public async Task<PaginatedResult<GetAllBookingResponse>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            IEnumerable<Domain.Entities.Booking.Booking> bookings;

            if (request.CinemaId > 0)
            {
                var ExistCinema = _cinemaRepository.Entities.FirstOrDefault(_ => !_.IsDeleted && _.Id == request.CinemaId);
                if (ExistCinema == null)
                    return PaginatedResult<GetAllBookingResponse>.Failure(new List<string> { "NOT_FOUND_CINEMA" });

                bookings = _bookingRepository.GetBookingsByCinemaAsync(request.CinemaId).AsQueryable();
            }
            else
                bookings = (from booking in _bookingRepository.Entities
                            where !booking.IsDeleted
                            select booking).AsQueryable();

            if (request.CustomerId > 0)
            {
                var ExistCustomer = _customerRepository.Entities.FirstOrDefault(_ => !_.IsDeleted && _.Id == request.CustomerId);
                if (ExistCustomer == null)
                    return PaginatedResult<GetAllBookingResponse>.Failure(new List<string> { StaticVariable.NOT_FOUND_CUSTOMER });

                bookings = from booking in bookings
                           where booking.CustomerId == request.CustomerId &&
                           booking.BookingMethod == BookingMethod.Online
                           select booking;

            }
            else
            {
                if (request.BookingMethod == GetAllBookingQuery.BookingMethodOption.Online)
                    bookings = from booking in bookings
                               where booking.BookingMethod == BookingMethod.Online
                               select booking;
                else if (request.BookingMethod == GetAllBookingQuery.BookingMethodOption.Offline)
                    bookings = from booking in bookings
                               where booking.BookingMethod == BookingMethod.Offline
                               select booking;
            }

            if (request.PaymentStatus == GetAllBookingQuery.PaymentStatusOption.Waiting)
                bookings = from booking in bookings
                           where booking.Status == _enumService.GetEnumIdByValue(StaticVariable.WAITING, StaticVariable.BOOKING_STATUS_ENUM)
                           select booking;
            else if (request.PaymentStatus == GetAllBookingQuery.PaymentStatusOption.Done)
                bookings = from booking in bookings
                           where booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                           select booking;

            var query = from booking in bookings
                        join user in _userManager.Users.AsEnumerable()
                        on new { UserId = booking.CustomerId, UserName = booking.CreatedBy, IsDeleted = false }
                        equals new { user.UserId, user.UserName, user.IsDeleted }
                        where string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(user.FullName, request.Keyword)
                                || booking.Id.ToString().Contains(request.Keyword)
                                || user.PhoneNumber.Contains(request.Keyword)
                        select new GetAllBookingResponse
                        {
                            Id = booking.Id,
                            BookingRefId = booking.BookingRefId,
                            CustomerName = user.FullName,
                            PhoneNumber = user.PhoneNumber,
                            ScheduleId = booking.ScheduleId,
                            TotalPrice = booking.RequiredAmount,
                            BookingDate = booking.BookingDate,
                            FilmName = _scheduleRepository.GetFilmName(booking.ScheduleId),
                            CinemaName = _scheduleRepository.GetCinemaName(booking.ScheduleId),
                            UsageStatus = booking.BookingStatus,
                            CreatedOn = booking.CreatedOn,
                            LastModifiedOn = booking.LastModifiedOn,
                        };

            var data = query.AsQueryable()
                .Where(x => string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.FilmName, request.Keyword)
                                || StringHelper.Contains(x.CinemaName, request.Keyword)
                                || StringHelper.Contains(x.CustomerName, request.Keyword)
                                || StringHelper.Contains(x.PhoneNumber, request.Keyword))
                .OrderBy(request.OrderBy).ToList();

            foreach (var booking in data)
            {
                if (string.IsNullOrEmpty(booking.UsageStatus))
                {
                    booking.UsageStatus = _bookingRepository.GetBookingUsageStatus(booking.Id);
                }
            }

            var totalRecord = data.Count();

            List<GetAllBookingResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();

            return PaginatedResult<GetAllBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}