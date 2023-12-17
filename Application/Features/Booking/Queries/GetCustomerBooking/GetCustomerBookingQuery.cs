﻿using Application.Features.Cinema.Queries.GetAll;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : RequestParameter, IRequest<PaginatedResult<GetCustomerBookingResponse>>
    {
        [Required]
        public long CustomerId { get; set; }
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, PaginatedResult<GetCustomerBookingResponse>>
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

        public async Task<PaginatedResult<GetCustomerBookingResponse>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            if (userId != request.CustomerId)
                return PaginatedResult<GetCustomerBookingResponse>.Failure(new List<string> { StaticVariable.NOT_FOUND_CUSTOMER});

            var bookings = await _bookingRepository.Entities.Where(
                _ => !_.IsDeleted && 
                _.CustomerId == request.CustomerId &&
                _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)) 
                .Select(s => new GetCustomerBookingResponse
                {
                    Id = s.Id,
                    BookingRefId = s.BookingRefId,
                    BookingDate = s.BookingDate,
                    TotalPrice = s.RequiredAmount,
                    BookingCurrency = s.BookingCurrency
                }).ToListAsync();
            foreach (var booking in bookings)
            {
                booking.FilmName = GetFilmNameByBooking(booking.Id);
                booking.CinemaName = GetCinemaNameByBooking(booking.Id);
            }

            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            List<GetCustomerBookingResponse> result = bookings.Where(x => string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.FilmName, request.Keyword)
                                || StringHelper.Contains(x.CinemaName, request.Keyword)).OrderBy(x =>x.BookingDate).ToList();

            var totalRecord = result.Count();

            if (!request.IsExport)
                result = result.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

            return PaginatedResult<GetCustomerBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
        public string? GetFilmNameByBooking(long BookingId)
        {
            var filmName = (from booking in _bookingRepository.Entities
                            where !booking.IsDeleted && booking.Id == BookingId
                            join schedule in _scheduleRepository.Entities
                            on booking.ScheduleId equals schedule.Id
                            where !schedule.IsDeleted 
                            join film in _filmRepository.Entities
                            on schedule.FilmId equals film.Id
                            where !film.IsDeleted
                            select film.Name).FirstOrDefault();
            return filmName;
        }
        public string? GetCinemaNameByBooking(long BookingId)
        {
            var cinemaName = (from booking in _bookingRepository.Entities
                              where !booking.IsDeleted && booking.Id == BookingId
                              join schedule in _scheduleRepository.Entities
                              on booking.ScheduleId equals schedule.Id
                              where !schedule.IsDeleted
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