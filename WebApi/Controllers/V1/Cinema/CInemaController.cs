using Application.Features.Cinema.Command.AddCinema;
using Application.Features.Cinema.Command.DeleteCinema;
using Application.Features.Cinema.Command.EditCinema;
using Application.Features.Cinema.Queries.GetAll;
using Application.Features.Cinema.Queries.GetById;
using Application.Features.Film.Command.AddFilm;
using Application.Features.Film.Command.DeleteFilm;
using Application.Features.Film.Queries.GetAll;
using Application.Features.Film.Queries.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Cinema
{
    [ApiController]

    [Route("api/v{version:apiVersion}/cinema")]
    public class CinemaController : BaseApiController<CinemaController>
    {
        /// <summary>
        /// Get Customer detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetCinemaByIdResponse>>> GetCinemaById(long id)
        {
            return Ok(await Mediator.Send(new GetCinemaByIdQuery()
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCinema(AddCinemaCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// 
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllCinemaResponse>>> GetAllFilm([FromQuery] GetAllCinemaQuery query)
        {
            return Ok(await Mediator.Send(new GetAllCinemaQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                City = query.City,
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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditCinema(EditCinemaCommand command)
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
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteCinema(long Id)
        {
            var result = await Mediator.Send(new DeleteCinemaCommand
            {
                Id = Id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
