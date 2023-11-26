using Application.Features.Merchant.Command;
using Application.Features.Merchant.Queries.GetAll;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;

namespace WebApi.Controllers.V1.Merchant
{
    [ApiController]

    [Route("api/v{version:apiVersion}/merchant")]
    public class MerchantController : BaseApiController<MerchantController>
    {
        /// <summary>
        /// Add/Edit customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMerchant(AddMerchantCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllMerchantResponse>>> GetAllMerchant([FromQuery] GetAllMerchantQuery query)
        {
            return Ok(await Mediator.Send(new GetAllMerchantQuery()
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
        [HttpPut]
        public async Task<IActionResult> EditMerchant(EditMerchantCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }


        ///// <summary>
        ///// Delete customer
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet]
        [Route("test-create-qrcode")]
        public async Task<IActionResult> Test()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);

            // cach 3
            //Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            //var imgType = Base64QRCode.ImageType.Jpeg;
            //string qrCodeImageAsBase64 = qrCode.GetGraphic(20);
            //var htmlPictureTag = $"<img alt=\"Embedded QR Code\" src=\"data:image/{imgType.ToString().ToLower()};base64,{qrCodeImageAsBase64}\" />";

            //cach 2:
            //AsciiQRCode qrCode = new AsciiQRCode(qrCodeData);
            //string qrCodeAsAsciiArt = qrCode.GetGraphic(1);

            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return Ok(qrCodeImage);
        }
    }
}
