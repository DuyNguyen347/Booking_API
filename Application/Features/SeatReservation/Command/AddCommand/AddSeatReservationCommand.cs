﻿using Application.Features.Booking.Command.AddBooking;
using Application.Features.Schedule.Command;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using Application.Interfaces.Ticket;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SeatReservation.Command.AddCommand
{
    public class AddSeatReservationCommand : IRequest<Result<AddSeatReservationCommand>>
    {
        public long CustomerId { get; set; }
        public long ScheduleId { get; set; }
        public List<int> NumberSeats { get; set; }
    }
    public class AddSeatReservationCommandHandler : IRequestHandler<AddSeatReservationCommand, Result<AddSeatReservationCommand>>
    {
        private readonly ISeatReservationService _seatReservationService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IEnumService _enumService;
        private readonly ITimeZoneService _timeZoneService;
        public AddSeatReservationCommandHandler(
            ISeatReservationService seatReservationService,
            ICustomerRepository customerRepository,
            IScheduleRepository scheduleRepository,
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository,
            IEnumService enumService,
            ITimeZoneService timeZoneService)
        {
            _seatReservationService = seatReservationService;
            _customerRepository = customerRepository;
            _scheduleRepository = scheduleRepository;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _enumService = enumService;
            _timeZoneService = timeZoneService;
        }
        public async Task<Result<AddSeatReservationCommand>> Handle(AddSeatReservationCommand request, CancellationToken cancellationToken)
        {
            var existCustomer = await _customerRepository.FindAsync(x => x.Id == request.CustomerId && !x.IsDeleted);
            if (existCustomer == null) return await Result<AddSeatReservationCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);
            var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
            if (existSchedule == null) return await Result<AddSeatReservationCommand>.FailAsync("NOT_FOUND_SCHEDULE");

            var existTickets = await (from booking in _bookingRepository.Entities
                                      where !booking.IsDeleted && booking.ScheduleId == request.ScheduleId
                                      where booking.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                                      || booking.ExpireDate > _timeZoneService.GetGMT7Time()
                                      join ticket in _ticketRepository.Entities
                                      on booking.Id equals ticket.BookingId
                                      where !ticket.IsDeleted 
                                      select ticket.NumberSeat).ToListAsync();
            foreach (var ticket in existTickets)
            {
                if (request.NumberSeats.Contains(ticket))
                    return await Result<AddSeatReservationCommand>.FailAsync("EXISTING_BOOKED_NUMBERSEATS");
            }

            bool IsSuccess = _seatReservationService.LockSeats(request.CustomerId, request.ScheduleId, request.NumberSeats);
            if (!IsSuccess)
                return Result<AddSeatReservationCommand>.Fail("SEATS_UNAVAILABLE_TEMPORARILY");
            return Result<AddSeatReservationCommand>.Success(request, "RESERVED_SUCCESSFULLY");
        }
    }
}
