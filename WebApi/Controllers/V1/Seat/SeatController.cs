using Application.Features.Room.Command.AddRoom;
using Application.Features.Room.Command.DeleteRoom;
using Application.Features.Room.Command.EditRoom;
using Application.Features.Room.Queries.GetAll;
using Application.Features.Room.Queries.GetById;
using Application.Features.Seat.Command;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Seat
{
    [ApiController]

    [Route("api/v{version:apiVersion}/Seat")]
    public class SeatController : BaseApiController<SeatController>
    {
        /// <summary>
        /// Edit Customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut]
        public async Task<IActionResult> ChangeStatusSeats(ChangeStatusSeatCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
