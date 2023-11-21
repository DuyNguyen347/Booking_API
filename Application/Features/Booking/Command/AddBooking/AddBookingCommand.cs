using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Merchant;
using Application.Interfaces.Payment;
using Application.Interfaces.Repositories;
using Application.Interfaces.Schedule;
using Application.Interfaces.Seat;
using Application.Interfaces.Services;
using Application.Interfaces.Ticket;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.AddBooking
{
    public class AddBookingCommand : IRequest<Result<AddBookingCommand>>
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long ScheduleId { get; set; }
        public List<int> NumberSeats { get; set; }
        public string? PaymentDestinationId { get; set; } = string.Empty;
    }

    internal class AddBookingCommandHandler : IRequestHandler<AddBookingCommand, Result<AddBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ISeatReservationService _seatReservationService;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IVnPayService _vnPayService;
        private readonly IMerchantRepository _merchantRepository;

        public AddBookingCommandHandler(
            IMapper mapper,
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ITicketRepository ticketRepository,
            ISeatRepository seatRepository,
            ISeatReservationService seatReservationService,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork,
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService,
            IVnPayService vnPayService,
            IMerchantRepository merchantRepository,
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _seatReservationService = seatReservationService;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _vnPayService = vnPayService;
            _merchantRepository = merchantRepository;
        }

        public async Task<Result<AddBookingCommand>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
        {
            //if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            //{
            //    long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            //    if (userId != request.CustomerId)
            //        return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            //}

            foreach (var NumberSeat in request.NumberSeats)
            {
                if (!_seatReservationService.ValidateLock(request.CustomerId, request.ScheduleId, NumberSeat))
                    return await Result<AddBookingCommand>.FailAsync("RESERVATION_TIME_OUT");
            }

            //open transaction
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var ExistCustomer = await _customerRepository.FindAsync(x => x.Id == request.CustomerId && !x.IsDeleted);
                if (ExistCustomer == null) return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

                var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
                if (existSchedule == null) return await Result<AddBookingCommand>.FailAsync("NOT_FOUND_SCHEDULE");

                var booking = new Domain.Entities.Booking.Booking()
                {
                    CustomerId = request.CustomerId,
                    ScheduleId = request.ScheduleId
                };
                //Them QR CODE
                //
                booking.Status = _enumService.GetEnumIdByValue(StaticVariable.WAITING, StaticVariable.BOOKING_STATUS_ENUM);
                await _bookingRepository.AddAsync(booking);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = booking.Id;

                var listSeats = await (from seat in _seatRepository.Entities
                                       join schedule in _scheduleRepository.Entities on seat.RoomId equals schedule.RoomId
                                       where !seat.IsDeleted && request.NumberSeats.Contains(seat.NumberSeat)
                                       select new
                                       {
                                           seatInfo = seat,
                                           price = schedule.Price
                                       }).ToListAsync();



                List<Domain.Entities.Ticket.Ticket> tickets = new List<Domain.Entities.Ticket.Ticket>();
                var amount = 0;
                foreach (var seat in listSeats)
                {
                    tickets.Add(new Domain.Entities.Ticket.Ticket()
                    {
                        BookingId = request.Id,
                        Type = Domain.Constants.Enum.TypeTicket.Normal,
                        Price = seat.price,
                        NumberSeat = seat.seatInfo.NumberSeat,
                        SeatCode = seat.seatInfo.SeatCode
                    });
                    amount += seat.price;
                }

                string contentPayment = _currentUserService.UserName + " tt " + string.Join(", ", request.NumberSeats);
                booking.BookingContent = contentPayment;
                booking.BookingCurrency = amount.ToString();
                booking.ExpireDate = DateTime.Now.AddMinutes(15);
                booking.BookingLanguage = "vn";
                booking.MerchantId = 1;
                booking.BookingCurrency = "VND";
                booking.BookingRefId = booking.Id.ToString();

                Console.WriteLine(_currentUserService.IpAddress);
                var paymentUrl = string.Empty;
                var merchant = await _merchantRepository.Entities.FirstOrDefaultAsync();
                switch (request.PaymentDestinationId)
                {
                    case "VNPAY":
                        _vnPayService.Init(DateTime.Now, _currentUserService.IpAddress ?? string.Empty, amount * 100, "VND",
                                "other", contentPayment ?? string.Empty,booking.Id.ToString() ?? string.Empty);
                        paymentUrl = _vnPayService.GetLink(_currentUserService.HostServerName);
                        booking.MerchantId = merchant.Id;
                        break;
                    default:
                        break;
                }

                await _bookingRepository.UpdateAsync(booking);
                Console.WriteLine(paymentUrl.ToString());
                await _ticketRepository.AddRangeAsync(tickets);
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await Result<AddBookingCommand>.SuccessAsync(paymentUrl.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("error create booking : ", ex.ToString());
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}