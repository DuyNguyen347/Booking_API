using Application.Features.Film.Command.AddFilm;
using Application.Features.Film.Command.DeleteFilm;
using Application.Features.Room.Command.EditRoom;
using Application.Features.Room.Queries.GetAll;
using Application.Features.Schedule.Command.AddSchedule;
using Application.Features.Schedule.Command.DeleteSchedule;
using Application.Features.Schedule.Command.EditSchedule;
using Application.Features.Schedule.Query.GetAll.GetAll;
using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Features.Schedule.Query.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.V1.Film;

namespace WebApi.Controllers.V1.Schedule
{
    [ApiController]
    [Route("api/v{version:apiVersion}/schedule")]
    public class ScheduleController: BaseApiController<ScheduleController>
    {
        [HttpGet("film/{id}")]
        public async Task<ActionResult<Result<Dictionary<string, Dictionary<string, List<GetAllScheduleByFilmResponse>>>>>> GetAllScheduleByFilm(long Id)
        {
            return Ok(await Mediator.Send(new GetAllScheduleByFilmQuery()
            {
                FilmId = Id
            }));
        }
        [HttpGet("cinema/{id}")]
        public async Task<ActionResult<ScheduleResult<GetAllScheduleByCinemaResponse>>> GetAllScheduleByCinema(long Id)
        {
            return Ok(await Mediator.Send(new GetAllScheduleByCinemaQuery()
            {
                CinemaId = Id
            }));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetScheduleByIdResponse>>> GetScheduleById(long Id)
        {
            return Ok(await Mediator.Send(new GetScheduleByIdQuery()
            {
                Id = Id
            }));
        }
        [HttpPost]
        public async Task<IActionResult> AddSchedule(AddScheduleCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
        [HttpPut]
        public async Task<IActionResult> EditSchedule(EditScheduleCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
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
