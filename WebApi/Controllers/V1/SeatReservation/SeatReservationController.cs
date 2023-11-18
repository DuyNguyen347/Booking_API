using Application.Features.Booking.Command.AddBooking;
using Application.Features.SeatReservation.Command;
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
    }
}
