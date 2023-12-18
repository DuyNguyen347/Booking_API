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
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Booking;

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
            var merchant = await _merchantRepository.Entities.FirstOrDefaultAsync();
            if (merchant == null) return await Result<ProcessVnPayReturnResponse>.FailAsync("NOT_FOUND_MERCHANT");
            string returnUrl = merchant.MerchantReturnUrl ?? string.Empty;
            try
            {
                //var isValidSign = request.IsValidSignature();
                if(request.vnp_ResponseCode.Equals("00") && request.vnp_TransactionStatus.Equals("00"))
                {
                    result.Amount = request.vnp_Amount;
                    result.PaymentDate = request.vnp_PayDate;
                    var booking = await _bookingRepository.Entities.Where(x => x.BookingRefId == request.vnp_TxnRef).FirstOrDefaultAsync();
                    if (booking != null)
                    {
                        result.PaymentStatus = "00";
                        booking.Status = (int)BookingStatus.Done;
                        result.Signature = Guid.NewGuid().ToString();
                        result.PaymentMessage = "Payment Success";
                        result.PaymentId = booking.BookingRefId;
                        

                        string data = $"{result.PaymentId}"; 
                        QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                        QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(data,QRCodeGenerator.ECCLevel.Q);

                        Base64QRCode qrCode = new Base64QRCode(qRCodeData);
                        //string qrCodeImageAsBase64 = qrCode.GetGraphic(10);
                        string qrCodeImageAsBase64 = "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";
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
               
                return await Result<ProcessVnPayReturnResponse>.FailAsync(result,returnUrl);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result.PaymentStatus = "10";
                result.PaymentMessage = "Payment Failed";
                return await Result<ProcessVnPayReturnResponse>.FailAsync(result,returnUrl);
            }
        }
    }
}
