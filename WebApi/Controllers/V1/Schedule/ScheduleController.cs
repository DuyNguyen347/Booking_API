using Application.Features.Schedule.Command.AddSchedule;
using Application.Features.Schedule.Command.AddScheduleCinemas;
using Application.Features.Schedule.Command.DeleteSchedule;
using Application.Features.Schedule.Command.EditSchedule;
using Application.Features.Schedule.Query.GetAll.GetAll;
using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Features.Schedule.Query.GetAll.GetAllSchedule;
using Application.Features.Schedule.Query.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Constants;

namespace WebApi.Controllers.V1.Schedule
{
    [ApiController]
    [Route("api/v{version:apiVersion}/schedule")]
    public class ScheduleController: BaseApiController<ScheduleController>
    {
        [HttpGet("film/{id}")]
        public async Task<ActionResult<Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>>> GetAllScheduleByFilm(long id)
        {
            return Ok(await Mediator.Send(new GetAllScheduleByFilmQuery()
            {
                FilmId = id
            }));
        }

        [HttpGet("cinema/{id}")]
        public async Task<ActionResult<Result<List<GetAllScheduleByCinemaResponse>>>> GetAllScheduleByCinema(long id)
        {
            return Ok(await Mediator.Send(new GetAllScheduleByCinemaQuery()
            {
                CinemaId = id
            }));
        }

        [HttpGet]
        public async Task<ActionResult<Result<List<GetAllScheduleResponse>>>> GetAllSchedule([FromQuery] GetAllScheduleQuery query)
        {
            return Ok(await Mediator.Send(new GetAllScheduleQuery()
            {
                CinemaId = query.CinemaId
            })) ;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetScheduleByIdResponse>>> GetScheduleById(long id)
        {
            return Ok(await Mediator.Send(new GetScheduleByIdQuery()
            {
                Id = id
            }));
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPost]
        public async Task<IActionResult> AddSchedule(AddScheduleCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPut]
        public async Task<IActionResult> EditSchedule(EditScheduleCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteSchedule(long Id)
        {
            var result = await Mediator.Send(new DeleteScheduleCommand
            {
                Id = Id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPost("cinemas")]
        public async Task<ActionResult<Result<AddScheduleMultipleCinemasResponse>>> AddScheduleCinemas (AddScheduleMultipleCinemasCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
