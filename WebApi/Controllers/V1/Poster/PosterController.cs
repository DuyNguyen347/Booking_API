using Application.Features.Poster.Command.AddPoster;
using Application.Features.Poster.Command.DeletePoster;
using Application.Features.Poster.Queries.GetAll;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Poster
{
    [ApiController]

    [Route("api/v{version:apiVersion}/Poster")]
    public class PosterController : BaseApiController<PosterController>
    {

        /// <summary>
        /// Add/Edit customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPoster(AddPosterCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllPosterReponse>>> GetAllFilm([FromQuery] GetAllPosterQuery query)
        {
            return Ok(await Mediator.Send(new GetAllPosterQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                OrderBy = query.OrderBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }));
        }


        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeletePoster(long Id)
        {
            var result = await Mediator.Send(new DeletePosterCommand
            {
                Id = Id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
