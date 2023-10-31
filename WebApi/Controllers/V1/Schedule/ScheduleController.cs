using Application.Features.Film.Command.AddFilm;
using Application.Features.Film.Command.DeleteFilm;
using Application.Features.Schedule.Command.AddSchedule;
using Application.Features.Schedule.Command.DeleteSchedule;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.V1.Film;

namespace WebApi.Controllers.V1.Schedule
{
    [ApiController]
    [Route("api/v{version:apiVersion}/schedule")]
    public class ScheduleController: BaseApiController<ScheduleController>
    {
        [HttpPost]
        public async Task<IActionResult> AddSchedule(AddScheduleCommand command)
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
