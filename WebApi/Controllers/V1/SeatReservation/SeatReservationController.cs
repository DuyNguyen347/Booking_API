using Application.Dtos.Responses;
using Application.Features.Booking.Command.AddBooking;
using Application.Features.Schedule.Query.GetAll.GetAll;
using Application.Features.SeatReservation.Command.AddCommand;
using Application.Features.SeatReservation.Command.UnlockCommand;
using Application.Features.SeatReservation.Query;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.V1.Booking;

namespace WebApi.Controllers.V1.SeatReservation
{
    [ApiController]
    [Route("api/v{version:apiVersion}/reserve")]
    public class SeatReservationController: BaseApiController<SeatReservationController>
    {
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSeatReservation(AddSeatReservationCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
        [HttpPost("unlock")]
        public async Task<IActionResult> UnlockSeatReservation(UnlockSeatReservationCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<Result<Dictionary<long, Dictionary<int, AddSeatReservationResponse>>>>> GetAllSeatReservation()
        {
            return Ok(await Mediator.Send(new GetAllSeatReservationQuery()));
        }
    }
}
