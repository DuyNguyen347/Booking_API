using Application.Dtos.Responses.Payment;
using Application.Features.Category.Command.AddCategory;
using Application.Features.Category.Queries.GetAll;
using Application.Features.Payment.Command;
using AutoMapper;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Payment
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : BaseApiController<PaymentController>
    {
        private readonly IMapper _mapper;
        public PaymentController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [Route("vnpay-return")]

        public async Task<IActionResult> VnpayReturn([FromQuery]VnPayReponse response)
        {
            ProcessVnPayReturnCommand t = _mapper.Map<ProcessVnPayReturnCommand>(response);
            var processResult = await Mediator.Send(t);
            var url = processResult.Messages[0];
            var model = processResult.Data;
           Console.WriteLine(processResult);

           // if (returnUrl.EndsWith("/"))
               // returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            return Redirect($"{url}?{model.ToQueryString()}");
            //return Ok();
        }

    }
}
