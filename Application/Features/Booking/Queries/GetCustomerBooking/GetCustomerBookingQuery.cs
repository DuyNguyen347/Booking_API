using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.Ticket;
using Application.Parameters;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Entities.Booking;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : RequestParameter, IRequest<Result<List<GetCustomerBookingResponse>>>
    {
        [Required]
        public long CustomerId { get; set; }
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, Result<List<GetCustomerBookingResponse>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly UserManager<AppUser> _userManager;

        public GetCustomerBookingQueryHandler(
            IBookingRepository bookingRepository,
            IEnumService enumService, 
            ICurrentUserService currentUserService,
            IScheduleRepository scheduleRepository,
            IFilmRepository filmRepository,
            IRoomRepository roomRepository,
            ICinemaRepository cinemaRepository,
            UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _currentUserService = currentUserService;
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _roomRepository = roomRepository;
            _cinemaRepository = cinemaRepository;
            _userManager = userManager;
        }

        public async Task<Result<List<GetCustomerBookingResponse>>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            if (userId != request.CustomerId)
                return await Result<List<GetCustomerBookingResponse>>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);

            var bookings = await _bookingRepository.Entities.Where(
                _ => !_.IsDeleted && 
                _.CustomerId == request.CustomerId &&
                _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)) 
                .Select(s => new GetCustomerBookingResponse
                {
                    Id = s.Id,
                    BookingRefId = s.BookingRefId,
                    BookingDate = s.BookingDate,
                    FilmName = GetFilmNameBySchedule(s.ScheduleId),
                    CinemaName = GetCinemaNameBySchedule(s.ScheduleId),
                    TotalPrice = s.RequiredAmount,
                    BookingCurrency = s.BookingCurrency
                }).ToListAsync();

            return await Result<List<GetCustomerBookingResponse>>.SuccessAsync(bookings);
        }
        private string? GetFilmNameBySchedule(long ScheduleId)
        {
            var filmName = (from schedule in _scheduleRepository.Entities
                            where !schedule.IsDeleted && schedule.Id == ScheduleId
                            join film in _filmRepository.Entities
                            on schedule.FilmId equals film.Id
                            where !film.IsDeleted
                            select film.Name).FirstOrDefault();
            return filmName;
        }
        private string? GetCinemaNameBySchedule(long ScheduleId)
        {
            var cinemaName = (from schedule in _scheduleRepository.Entities
                              where !schedule.IsDeleted && schedule.Id == ScheduleId
                              join room in _roomRepository.Entities
                              on schedule.RoomId equals room.Id
                              join cinema in _cinemaRepository.Entities
                              on room.CinemaId equals cinema.Id
                              where !room.IsDeleted && !cinema.IsDeleted
                              select cinema.Name).FirstOrDefault();
            return cinemaName;
        }
    }
}