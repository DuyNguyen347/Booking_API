using Application.Dtos.Responses;
using Application.Interfaces.Services;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SeatReservation.Query
{
    public class GetAllSeatReservationQuery: IRequest<Result<Dictionary<long, Dictionary<int, AddSeatReservationResponse>>>>
    {
    }
    public class GetAllSeatReservationHandler : IRequestHandler<GetAllSeatReservationQuery, Result<Dictionary<long, Dictionary<int, AddSeatReservationResponse>>>>
    {
        private readonly ISeatReservationService _seatReservationService;
        public GetAllSeatReservationHandler(ISeatReservationService seatReservationService) 
        {
            _seatReservationService = seatReservationService;
        }
        public async Task<Result<Dictionary<long, Dictionary<int, AddSeatReservationResponse>>>> Handle (GetAllSeatReservationQuery request, CancellationToken cancellationToken)
        {
            return await Result<Dictionary<long, Dictionary<int, AddSeatReservationResponse>>>.SuccessAsync(_seatReservationService.GetAll());
        }
    }
}
