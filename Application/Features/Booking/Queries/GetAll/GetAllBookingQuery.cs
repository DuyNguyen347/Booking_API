using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Parameters;
using Domain.Constants;
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

        public GetAllBookingHandler(
            IBookingRepository bookingRepository, 
            ICustomerRepository customerRepository,
            IEnumService enumService)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _enumService = enumService;
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
                            CustomerName = customer.CustomerName,
                            PhoneNumber = customer.PhoneNumber,
                            TotalPrice = booking.RequiredAmount,
                            BookingDate = booking.BookingDate
                        };

            var data = query.AsQueryable().OrderBy(request.OrderBy);
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