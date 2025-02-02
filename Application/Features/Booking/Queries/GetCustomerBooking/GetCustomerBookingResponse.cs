﻿using Application.Dtos.Responses.ServiceImage;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingResponse
    {
        public long Id { get; set; }
        public string? BookingRefId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? FilmName { get; set; }
        public string? CinemaName {  get; set; }
        public decimal? TotalPrice {  get; set; }
        public string? BookingCurrency { get; set; }
        public string? UsageStatus {  get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
