using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;

namespace Domain.Entities.Booking
{
    [Table("booking")]
    public class Booking : AuditableBaseEntity<long>
    {
        [Required]
        [Column("customer_id",TypeName = "bigint")]
        public long CustomerId { get; set; }

        [Required]
        [Column("schedule_id", TypeName = "bigint")]
        public long ScheduleId { get; set; }

        [Column("qr_code", TypeName = "varchar(max)")]
        public string? QRCode { get; set; }

        [Required]
        [Column("status", TypeName = "int")]
        public int? Status { get; set; }

        [Column("booking_content", TypeName = "nvarchar(max)")]
        public string BookingContent { get; set; } = string.Empty;

        [Column("booking_currency", TypeName = "nvarchar(20)")]
        public string BookingCurrency { get; set; } = string.Empty;

        [Column("booking_ref_id", TypeName = "varchar(max)")]
        public string BookingRefId { get; set; } = string.Empty;
        
        [Column("required_amount", TypeName = "decimal")]
        public decimal? RequiredAmount { get; set; }
        
        [Column("booking_date", TypeName = "datetime")]
        public DateTime? BookingDate { get; set; } = DateTime.Now;
        
        [Column("expire_date", TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }
        
        [Column("booking_language", TypeName = "varchar(50)")]
        public string? BookingLanguage { get; set; } = string.Empty;
        
        [Column("merchant_id", TypeName = "varchar(max)")]
        public long MerchantId { get; set; }

        [Column("booking_destination_id", TypeName = "varchar(60)")]
        public string? BookingDestinationId { get; set; } = string.Empty;
        
        [Column("paid_amount", TypeName = "decimal")]
        public decimal? PaidAmount { get; set; }
        
        [Column("booking_status", TypeName = "nvarchar(30)")]
        public string? BookingStatus { get; set; } = string.Empty;
        
        [Column("booking_message", TypeName = "varchar(60)")]
        public string? BookingMessage { get; set; } = string.Empty;
    }
}
