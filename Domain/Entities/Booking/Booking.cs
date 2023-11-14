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

        [Required]
        [Column("qr_code", TypeName = "varbinary(max)")]
        public byte[]? QRCode { get; set; }

        [Required]
        [Column("status", TypeName = "int")]
        public int? Status { get; set; }

    }
}
