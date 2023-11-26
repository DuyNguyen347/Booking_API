using Application.Dtos.Responses.Payment;
using Application.Exceptions;
using Application.Features.Booking.Command.AddBooking;
using Application.Interfaces.Payment;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Schedule;
using Application.Interfaces.Seat;
using Application.Interfaces.Services;
using Application.Interfaces.Ticket;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Domain.Constants.Enum;
using Application.Interfaces.Merchant;
using QRCoder;
using System.Drawing;
using iTextSharp.text.html.simpleparser;

namespace Application.Features.Payment.Command
{
    public class ProcessVnPayReturnCommand : VnPayReponse, IRequest<Result<ProcessVnPayReturnResponse>>
    {

    }

    internal class ProcessVnPayReturnCommandHandler : IRequestHandler<ProcessVnPayReturnCommand, Result<ProcessVnPayReturnResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IVnPayService _vnPayService;
        private readonly IMerchantRepository _merchantRepository;

        public ProcessVnPayReturnCommandHandler(
            IMapper mapper,
            IBookingRepository bookingRepository,
            IUnitOfWork<long> unitOfWork,
            ICurrentUserService currentUserService,
            IVnPayService vnPayService,
            IMerchantRepository merchantRepository,
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _vnPayService = vnPayService;
            _merchantRepository = merchantRepository;
        }

        public async Task<Result<ProcessVnPayReturnResponse>> Handle(ProcessVnPayReturnCommand request, CancellationToken cancellationToken)
        {
            var result = new ProcessVnPayReturnResponse() { };
            string returnUrl;
            try
            {
                //var isValidSign = request.IsValidSignature();
                if(request.vnp_ResponseCode.Equals("00") && request.vnp_TransactionStatus.Equals("00"))
                {
                    result.Amount = request.vnp_Amount;
                    result.PaymentDate = request.vnp_PayDate;
                    var booking = await _bookingRepository.GetByIdAsync(long.Parse(request.vnp_TxnRef));
                    if (booking != null)
                    {
                        result.PaymentStatus = "00";
                        booking.Status = (int)BookingStatus.Done;
                        result.Signature = Guid.NewGuid().ToString();
                        result.PaymentMessage = "Payment Success";
                        result.PaymentId = booking.Id.ToString();
                        var merchant = await _merchantRepository.GetByIdAsync(booking.MerchantId);
                        returnUrl = merchant.MerchantReturnUrl ?? string.Empty;

                        string data = $"{result.PaymentId},{result.Amount},{result.PaymentMessage},{result.PaymentStatus},{result.PaymentDate}"; 
                        QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                        QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(data,QRCodeGenerator.ECCLevel.Q);

                        Base64QRCode qrCode = new Base64QRCode(qRCodeData);
                        string qrCodeImageAsBase64 = qrCode.GetGraphic(20);
                        result.QRCode = qrCodeImageAsBase64;
                        booking.QRCode = qrCodeImageAsBase64;
                        await _bookingRepository.UpdateAsync(booking);
                        await _unitOfWork.Commit(cancellationToken);
                        
                        return await Result<ProcessVnPayReturnResponse>.SuccessAsync(result, returnUrl);
                    }
                    else
                    {
                        result.PaymentStatus = "11";
                        result.PaymentMessage = "Payment Failed - Unknown Payment";
                    }
                }
                else
                {
                    result.PaymentStatus = "10";
                    result.PaymentMessage = "Payment Failed";
                }
               
                return await Result<ProcessVnPayReturnResponse>.FailAsync(result,"");
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result.PaymentStatus = "10";
                result.PaymentMessage = "Payment Failed";
                return await Result<ProcessVnPayReturnResponse>.FailAsync(result,"");
            }
        }
    }
}
