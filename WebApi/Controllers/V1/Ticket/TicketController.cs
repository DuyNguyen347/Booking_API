//using Domain.Wrappers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace WebApi.Controllers.V1.Ticket
//{
//    [ApiController]

//    [Route("api/v{version:apiVersion}/Ticket")]
//    public class TicketController : BaseApiController<TicketController>
//    {
    //    /// <summary>
    //    /// Get Customer detail by Id
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    //[Authorize]
    //    [HttpGet("{id}")]
    //    public async Task<ActionResult<Result<GetTicketByIdResponse>>> GetTicketById(long id)
    //    {
    //        return Ok(await Mediator.Send(new GetTicketByIdQuery()
    //        {
    //            Id = id
    //        }));
    //    }

        /// <summary>
        /// Add/Edit customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        //[HttpPost]
        //public async Task<IActionResult> AddTicket(AddTicketCommand command)
        //{
        //    var result = await Mediator.Send(command);
        //    return (result.Succeeded) ? Ok(result) : BadRequest(result);
        //}

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        //[HttpGet]
        //public async Task<ActionResult<PaginatedResult<GetAllTicketResponse>>> GetAllTicket([FromQuery] GetAllTicketQuery query)
        //{
        //    return Ok(await Mediator.Send(new GetAllTicketQuery()
        //    {
        //        IsExport = query.IsExport,
        //        Keyword = query.Keyword,
        //        Status = query.Status,
        //        OrderBy = query.OrderBy,
        //        PageNumber = query.PageNumber,
        //        PageSize = query.PageSize
        //    }));
        //}

        /// <summary>
        /// Edit Customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPut]
        //public async Task<IActionResult> EditTicket(EditTicketCommand command)
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
        //[HttpDelete]
        //public async Task<IActionResult> DeleteTicket(long Id)
        //{
        //    var result = await Mediator.Send(new DeleteTicketCommand
        //    {
        //        Id = Id
        //    });
        //    return (result.Succeeded) ? Ok(result) : BadRequest(result);
        //}
//    }
//}

