using Application.Features.Room.Command.AddRoom;
using Application.Features.Room.Command.DeleteRoom;
using Application.Features.Room.Command.EditRoom;
using Application.Features.Room.Queries.GetAll;
using Application.Features.Room.Queries.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Room
{
    [ApiController]

    [Route("api/v{version:apiVersion}/Room")]
    public class RoomController : BaseApiController<RoomController>
    {
        /// <summary>
        /// Get Customer detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetRoomByIdResponse>>> GetRoomById(long id)
        {
            return Ok(await Mediator.Send(new GetRoomByIdQuery()
            {
                Id = id
            }));
        }

        /// <summary>
        /// Add/Edit customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPost]
        public async Task<IActionResult> AddRoom(AddRoomCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllRoomResponse>>> GetAllRoom([FromQuery] GetAllRoomQuery query)
        {
            return Ok(await Mediator.Send(new GetAllRoomQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                Status = query.Status,
                OrderBy = query.OrderBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }));
        }

        /// <summary>
        /// Edit Customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut]
        public async Task<IActionResult> EditRoom(EditRoomCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(long Id)
        {
            var result = await Mediator.Send(new DeleteRoomCommand
            {
                Id = Id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
