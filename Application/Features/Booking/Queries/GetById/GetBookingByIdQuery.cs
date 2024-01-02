using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Cinema;
using Application.Interfaces.Customer;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Service;
using Application.Interfaces.Ticket;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Entities.Films;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Queries.GetById
{
    public class GetBookingByIdQuery : IRequest<Result<GetBookingByIdResponse>>
    {
        public string PaymentId { get; set; }
    }
    internal class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        private readonly IFilmImageRepository _filmImageRepository;
        private readonly IEnumService _enumService;
        private readonly UserManager<AppUser> _userManager;

        public GetBookingByIdQueryHandler(
            IMapper mapper,
            IBookingRepository bookingRepository, 
            ITicketRepository ticketRepository,
            ICustomerRepository customerRepository, 
            IScheduleRepository scheduleRepository,
            IRoomRepository roomRepository,
            ICinemaRepository cinemaRepository,
            IFilmRepository filmRepository,
            IUploadService uploadService,
            IFilmImageRepository filmImageRepository, 
            IEnumService enumService,
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _customerRepository = customerRepository;
            _scheduleRepository = scheduleRepository;
            _roomRepository = roomRepository;
            _cinemaRepository = cinemaRepository;
            _filmRepository = filmRepository;
            _uploadService = uploadService;
            _filmImageRepository = filmImageRepository;
            _enumService = enumService;
            _userManager = userManager;
        }
        public async Task<Result<GetBookingByIdResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {

            //if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            //{
            //    long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            //    if (userId != Booking.CustomerId)
            //        return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            //}

            var Booking = await _bookingRepository.Entities
                .Where(booking => booking.BookingRefId == request.PaymentId && !booking.IsDeleted)
                .Select(booking => booking).FirstOrDefaultAsync();
            if(Booking == null)
            {
                return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            var response = _mapper.Map<GetBookingByIdResponse>(Booking);

            var CustomerBooking = await _userManager.Users
                .Where(_ => !_.IsDeleted && _.UserId == Booking.CustomerId && _.UserName == Booking.CreatedBy)
                .Select(s => new 
                {
                    Name = s.FullName,
                    Phone = s.PhoneNumber,
                }).FirstOrDefaultAsync();

            response.CustomerName = CustomerBooking.Name;
            response.PhoneNumber = CustomerBooking.Phone;
            response.TotalPrice = Booking.RequiredAmount;

            var bookingInfo = await (from schedule in _scheduleRepository.Entities
                              where !schedule.IsDeleted && schedule.Id == Booking.ScheduleId
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
                                  Image = _uploadService.GetFullUrl(_filmImageRepository.Entities.Where(_ => !_.IsDeleted && _.FilmId == film.Id).Select(y => y.NameFile).FirstOrDefault()),
                              }).FirstOrDefaultAsync();

            if (bookingInfo != null)
            {
                response.CinemaName = bookingInfo.CinemaName;
                response.RoomName = bookingInfo.RoomName;
                response.FilmName = bookingInfo.FilmName;
                response.Image = bookingInfo.Image;
                response.StartTime = bookingInfo.StartTime;
            }

            response.UsageStatus = string.IsNullOrEmpty(Booking.BookingStatus) ?
                    _bookingRepository.GetBookingUsageStatus(response.Id) : Booking.BookingStatus;

            List<TicketBookingResponse> bookingTickets = await _ticketRepository.Entities
                .Where(_ => _.BookingId == Booking.Id && !_.IsDeleted)
                .Select(s => new TicketBookingResponse
                {
                    Id = s.Id,
                    TypeTicket = s.Type,
                    Price = s.Price,
                    NumberSeat = s.NumberSeat,
                    SeatCode = s.SeatCode
                }).ToListAsync();
            response.Tickets = bookingTickets;
            return await Result<GetBookingByIdResponse>.SuccessAsync(response);
        }
    }
}
