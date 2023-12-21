using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Schedule;
using Application.Parameters;
using Domain.Constants;
using Domain.Entities.Customer;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingQuery : RequestParameter, IRequest<PaginatedResult<GetAllBookingResponse>>
    {
    }

    internal class GetAllBookingHandler : IRequestHandler<GetAllBookingQuery, PaginatedResult<GetAllBookingResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEnumService _enumService;
        private readonly IScheduleRepository _scheduleRepository;

        public GetAllBookingHandler(
            IBookingRepository bookingRepository, 
            ICustomerRepository customerRepository,
            IEnumService enumService,
            IScheduleRepository scheduleRepository)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _enumService = enumService;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<PaginatedResult<GetAllBookingResponse>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = from booking in _bookingRepository.Entities.AsEnumerable()
                        join customer in _customerRepository.Entities.AsEnumerable() on booking.CustomerId equals customer.Id
                        where !booking.IsDeleted
                                && (string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(customer.CustomerName, request.Keyword)
                                || booking.Id.ToString().Contains(request.Keyword)
                                || customer.PhoneNumber.Contains(request.Keyword))
                        where booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                        select new GetAllBookingResponse
                        {
                            Id = booking.Id,
                            BookingRefId = booking.BookingRefId,
                            CustomerName = customer.CustomerName,
                            PhoneNumber = customer.PhoneNumber,
                            TotalPrice = booking.RequiredAmount,
                            BookingDate = booking.BookingDate,
                            FilmName = _scheduleRepository.GetFilmName(booking.ScheduleId),
                            CinemaName = _scheduleRepository.GetCinemaName(booking.ScheduleId),
                            CreatedOn = booking.CreatedOn,
                            LastModifiedOn = booking.LastModifiedOn,
                        };

            var data = query.AsQueryable()
                .Where(x => string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.FilmName, request.Keyword)
                                || StringHelper.Contains(x.CinemaName, request.Keyword))
                .OrderBy(request.OrderBy);
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