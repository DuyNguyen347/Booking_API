using Application.Features.Cinema.Queries.GetAll;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Parameters;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : RequestParameter, IRequest<PaginatedResult<GetCustomerBookingResponse>>
    {
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, PaginatedResult<GetCustomerBookingResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<AppUser> _userManager;

        public GetCustomerBookingQueryHandler(
            IBookingRepository bookingRepository,
            IEnumService enumService, 
            ICurrentUserService currentUserService,
            IScheduleRepository scheduleRepository,
            UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _currentUserService = currentUserService;
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
        }

        public async Task<PaginatedResult<GetCustomerBookingResponse>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            var query= _bookingRepository.Entities.AsEnumerable()
                .Where(
                _ => !_.IsDeleted &&
                _.CustomerId == userId &&
                _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
                .AsQueryable()
                .Select(s => new GetCustomerBookingResponse
                {
                    Id = s.Id,
                    BookingRefId = s.BookingRefId,
                    BookingDate = s.BookingDate,
                    TotalPrice = s.RequiredAmount,
                    BookingCurrency = s.BookingCurrency,
                    FilmName = _scheduleRepository.GetFilmName(s.ScheduleId),
                    CinemaName = _scheduleRepository.GetCinemaName(s.ScheduleId),
                    CreatedOn = s.CreatedOn,
                    LastModifiedOn = s.LastModifiedOn,
                });

            var orderedBookings = query.OrderBy(request.OrderBy).ToList();

            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var searchedBookings = orderedBookings.Where(x => string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.FilmName, request.Keyword)
                                || StringHelper.Contains(x.CinemaName, request.Keyword)).ToList();

            var totalRecord = searchedBookings.Count();
            List<GetCustomerBookingResponse> result;
            if (!request.IsExport)
                result = searchedBookings.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = searchedBookings;

            return PaginatedResult<GetCustomerBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}