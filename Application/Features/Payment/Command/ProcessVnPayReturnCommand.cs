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
using SkiaSharp;
using System.Net.NetworkInformation;
using Application.Dtos.Requests.SendEmail;
using Hangfire;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Room;

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
        private readonly IEmailService _mailService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IRoomRepository _roomRepository;

        public ProcessVnPayReturnCommandHandler(
            IMapper mapper,
            IBookingRepository bookingRepository,
            IUnitOfWork<long> unitOfWork,
            ICurrentUserService currentUserService,
            IVnPayService vnPayService,
            IMerchantRepository merchantRepository,
            IEmailService mailService,
            ICinemaRepository cinemaRepository,
            IScheduleRepository scheduleRepository,
            IFilmRepository filmRepository,
            IRoomRepository roomRepository,
            ICustomerRepository customerRepository,
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _vnPayService = vnPayService;
            _merchantRepository = merchantRepository;
            _mailService = mailService;
            _customerRepository = customerRepository;
            _cinemaRepository = cinemaRepository;
            _filmRepository = filmRepository;
            _roomRepository = roomRepository;
            _scheduleRepository = scheduleRepository;
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
                        QRCodeData qRCodeData = null;
                        using (QRCodeGenerator qRCodeGenerator = new QRCodeGenerator())
                        {
                            qRCodeData = qRCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                        }
                        var imgType = Base64QRCode.ImageType.Png;
                        Base64QRCode qrCode = new Base64QRCode(qRCodeData);
                        string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White,true,imgType);

                        result.QRCode = qrCodeImageAsBase64;
                        booking.QRCode = qrCodeImageAsBase64;
                        await _bookingRepository.UpdateAsync(booking);
                        await _unitOfWork.Commit(cancellationToken);
                      
                        try
                        {
                            var CustomerBooking = await _customerRepository.Entities.Where(_ => _.Id == booking.CustomerId).Select(s => new                               Domain.Entities.Customer.Customer
                            {
                                Id = s.Id,
                                CustomerName = s.CustomerName,
                                PhoneNumber = s.PhoneNumber,
                            }).FirstOrDefaultAsync();

                            var bookingInfo = await (from schedule in _scheduleRepository.Entities
                                                     where !schedule.IsDeleted && schedule.Id == booking.ScheduleId
                            join room in _roomRepository.Entities
                                                     on schedule.RoomId equals room.Id
                                                     join cinema in _cinemaRepository.Entities
                                                     on room.CinemaId equals cinema.Id
                                                     join film in _filmRepository.Entities
                                                     on schedule.FilmId equals film.Id
                                                     where !room.IsDeleted && !cinema.IsDeleted && !film.IsDeleted
                                                     select new
                                                     {
                                                         CinemaName = cinema.Name,
                                                         RoomName = room.Name,
                                                         FilmName = film.Name,
                                                         StartTime = schedule.StartTime,
                                                     }).FirstOrDefaultAsync();

                            var bodyhtml = $@"<!DOCTYPE html>
                            <html lang=""en"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Vé Đặt Lịch Xem Phim - Cinephile</title>
                                <style>
                                    body {{
                                        font-family: Arial, sans-serif;
                                        margin: 20px;
                                    }}

                                    .ticket {{
                                        border: 1px solid #ccc;
                                        padding: 20px;
                                        max-width: 400px;
                                        margin: 0 auto;
                                    }}

                                    .ticket img {{
                                        max-width: 100%;
                                        height: auto;
                                        margin-top: 10px;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class=""ticket"">
                                    <h2>Cinephile - Vé Đặt Lịch Xem Phim</h2>
                                    <p><strong>Rạp Chiếu:</strong> {bookingInfo.CinemaName}</p>
                                    <p><strong>Phòng Chiếu:</strong> {bookingInfo.RoomName}</p>
                                    <p><strong>Số Ghế:</strong>A1,A3</p>
                                    <p><strong>Tên Phim:</strong> {bookingInfo.FilmName}</p>
                                    <p><strong>Thời Gian Chiếu:</strong> {bookingInfo.StartTime}</p>
                                    <p><strong>Mã QR:</strong></p>
                                    <img src=""data:image/png;base64,{qrCodeImageAsBase64}"" alt=""QR Code"">
                                    <p>Vui lòng đem mã QR nay khi đến rạp chiếu phim</strong></p>
                                </div>
                            </body>
                            </html>";

                            var email = new EmailRequest()
                            {
                                Body = bodyhtml,
                                Subject = "Xác nhận thanh toán vé xem phim thành công",
                                To = "ducduynguyen347@gmail.com"
                            };
                            BackgroundJob.Enqueue(() => _mailService.SendAsync(email));
                        } catch(Exception ex)
                        {
                            Console.WriteLine("err when send email: ", ex.Message);
                        }
                        
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
