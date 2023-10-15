using Application.Features.Category.Command.AddCategory;
using Application.Features.Category.Queries.GetAll;
using Application.Features.Film.Command.AddFilm;
using Application.Features.Film.Command.DeleteFilm;
using Application.Features.Film.Queries.GetAll;
using Application.Features.Film.Queries.GetById;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Film
{
    [ApiController]

    [Route("api/v{version:apiVersion}/film")]
    public class FilmController : BaseApiController<FilmController>
    {
        /// <summary>
        /// Get Customer detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetFilmByIdReponse>>> GetCategoryById(long id)
        {
            return Ok(await Mediator.Send(new GetFilmByIdQuery()
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
        public async Task<IActionResult> AddFilm(AddFilmCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllFilmResponse>>> GetAllCustomer([FromQuery] GetAllFilmQuery query)
        {
            return Ok(await Mediator.Send(new GetAllFilmQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
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
        //[HttpPut]
        //public async Task<IActionResult> EditCustomer(EditCustomerCommand command)
        //{
        //    var result = await Mediator.Send(command);
        //    return (result.Succeeded) ? Ok(result) : BadRequest(result);
        //}

        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteFilm(long Id)
        {
            var result = await Mediator.Send(new DeleteFilmCommand
            {
                Id = Id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
