using Application.Features.Schedule.Command.AddSchedule;
using Application.Features.Schedule.Command.AddScheduleMultiCinemas;
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
using Application.Features.Schedule.Command.AddScheduleMultiTimeSlots;

namespace WebApi.Controllers.V1.Schedule
{
    [ApiController]
    [Route("api/v{version:apiVersion}/schedule")]
    public class ScheduleController: BaseApiController<ScheduleController>
    {
        //get schedule theo film id (customer view)
        [HttpGet("film/{id}")]
        public async Task<ActionResult<Result<Dictionary<string, Dictionary<long, List<GetAllScheduleByFilmResponse>>>>>> GetAllScheduleByFilm(long id)
        {
            return Ok(await Mediator.Send(new GetAllScheduleByFilmQuery()
            {
                FilmId = id
            }));
        }

        //get schedule theo cinema id (admin view)
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet("cinema/{id}")]
        public async Task<ActionResult<Result<List<GetAllScheduleByCinemaResponse>>>> GetAllScheduleByCinema(long id)
        {
            return Ok(await Mediator.Send(new GetAllScheduleByCinemaQuery()
            {
                CinemaId = id
            }));
        }


        //get tat ca schedule
        [HttpGet]
        public async Task<ActionResult<Result<List<GetAllScheduleResponse>>>> GetAllSchedule([FromQuery] GetAllScheduleQuery query)
        {
            return Ok(await Mediator.Send(new GetAllScheduleQuery()
            {
                CinemaId = query.CinemaId
            })) ;
        }

        //Get schedule theo schedule id
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetScheduleByIdResponse>>> GetScheduleById(long id)
        {
            return Ok(await Mediator.Send(new GetScheduleByIdQuery()
            {
                Id = id
            }));
        }

        //them single schedule
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPost]
        public async Task<IActionResult> AddSchedule(AddScheduleCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        //them schedule cho nhieu rap cung luc -  multi cinemas adding
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPost("cinemas")]
        public async Task<ActionResult<Result<AddScheduleMultiCinemasResponse>>> AddScheduleCinemas(AddScheduleMultiCinemasCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        //them schedule cho mot rap o nhieu khung gio - multi time slots
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPost("muli-time-slots")]
        public async Task<IActionResult> AddScheduleMultiTimeSlots(AddScheduleMultiTimeSlotsCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        //Chinh sua mot schedule
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPut]
        public async Task<IActionResult> EditSchedule(EditScheduleCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        //xoa mot schedule
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
    }
}
