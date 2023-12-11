using Application.Features.Category.Command.AddCategory;
using Application.Features.Category.Queries.GetAll;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Category
{
    [ApiController]

    [Route("api/v{version:apiVersion}/category")]
    public class CategoryController : BaseApiController<CategoryController>
    {
        ///// <summary>
        ///// Get Customer detail by Id
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Authorize]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Result<GetCategoryByIdResponse>>> GetCategoryById(long id)
        //{
        //    return Ok(await Mediator.Send(new GetCustomerByIdQuery()
        //    {
        //        Id = id
        //    }));
        //}

        /// <summary>
        /// Add/Edit customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllCategoryResponse>>> GetAllCategory([FromQuery] GetAllCategoryQuery query)
        {
            return Ok(await Mediator.Send(new GetAllCategoryQuery()
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
        ///// <summary>
        ///// Delete customer
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        //[HttpDelete]
        //public async Task<IActionResult> DeleteCustomer(long Id)
        //{
        //    var result = await Mediator.Send(new DeleteCustomerCommand
        //    {
        //        Id = Id
        //    });
        //    return (result.Succeeded) ? Ok(result) : BadRequest(result);
        //}
    }
}
