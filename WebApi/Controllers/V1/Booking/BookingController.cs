using Application.Features.Booking.Command.AddBooking;
using Application.Features.Booking.Command.DeleteBooking;
using Application.Features.Booking.Command.EditBooking;
using Application.Features.Booking.Command.UpdateStatusBooking;
using Application.Features.Booking.Command.UpdateUsageStatus;
using Application.Features.Booking.Queries.GetAll;
using Application.Features.Booking.Queries.GetById;
using Application.Features.Booking.Queries.GetCustomerBooking;
using Application.Features.Booking.Queries.GetCustomerBookingHistory;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Booking
{
    [ApiController]
    [Route("api/v{version:apiVersion}/booking")]
    public class BookingController : BaseApiController<BookingController>
    {
        /// <summary>
        /// Get all bookings pagination, filter
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllBookingResponse>>> GetAllBookings([FromQuery] GetAllBookingQuery query)
        {
            var result = await Mediator.Send(new GetAllBookingQuery()
            {
                CustomerId = query.CustomerId,
                BookingMethod = query.BookingMethod,
                PaymentStatus = query.PaymentStatus,
                CinemaId = query.CinemaId,
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                OrderBy = query.OrderBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            });
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add Booking
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBooking(AddBookingCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get Booking Detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{paymentId}")]
        public async Task<ActionResult<Result<GetBookingByIdResponse>>> BookingDetail(string paymentId)
        {   
            var result = await Mediator.Send(new GetBookingByIdQuery
            {
                PaymentId = paymentId
            });
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete Booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpDelete]
        //public async Task<IActionResult> DeleteBooking(short id)
        //{
        //    var result = await Mediator.Send(new DeleteBookingCommand
        //    {
        //        Id = id
        //    });
        //    return (result.Succeeded) ? Ok(result) : BadRequest(result);
        //}

        /// <summary>
        /// Edit Booking
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPut]
        //public async Task<IActionResult> EditBooking(EditBookingCommand command)
        //{
        //    var result = await Mediator.Send(command);
        //    return (result.Succeeded) ? Ok(result) : BadRequest(result);
        //}

        /// <summary>
        /// Update Status Booking
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatusBooking(UpdateStatusBookingCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get customer booking history
        /// </summary>
        /// <param name="idCustomer"></param>
        /// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        //[HttpGet("customer/{idCustomer}")]
        //public async Task<IActionResult> GetCustomerBookingHistory(long idCustomer)
        //{
        //    return Ok(await Mediator.Send(new GetCustomerBookingHistoryQuery
        //    {
        //        CustomerId = idCustomer
        //    }));
        //}

        /// <summary>
        /// Get customer booking
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomerBooking([FromQuery] GetCustomerBookingQuery query)
        {
            var result = await Mediator.Send(new GetCustomerBookingQuery
            {
                Keyword = query.Keyword,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                IsExport = query.IsExport,
                OrderBy = query.OrderBy,
            });
            return result.Succeeded? Ok(result): BadRequest(result);
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpPatch("update-usage-status")]
        public async Task<IActionResult> UpdateUsageStatusBooking(UpdateUsageStatusCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}