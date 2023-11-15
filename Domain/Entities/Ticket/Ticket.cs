using Domain.Constants.Enum;
using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Ticket
{
    [Table("ticket")]
    public class Ticket: AuditableBaseEntity<long>
    {
        [Required]
        [Column("type", TypeName = "int")]
        public TypeTicket Type { get; set; }

        [Required]
        [Column("price", TypeName = "int")]
        public int Price { get; set; }

        [Required]
        [Column("schedule_id", TypeName = "bigint")]
        public long ScheduleId { get; set; }

        [Required]
        [Column("user_id", TypeName = "bigint")]
        public long UserId { get; set; }

        [Required]
        [Column("seat_id", TypeName = "bigint")]
        public long SeatId { get; set; }

        [Column("qr_code", TypeName = "varbinary(max)")]
        public byte[]? QRCode { get; set; }
    }
}
