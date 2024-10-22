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
        [Column("booking_id", TypeName = "bigint")]
        public long BookingId { get; set; }

        [Required]
        [Column("type", TypeName = "int")]
        public TypeTicket Type { get; set; }

        [Required]
        [Column("price", TypeName = "int")]
        public int Price { get; set; }

        [Required]
        [Column("number", TypeName = "int")]
        public int NumberSeat { get; set; }

        [Required]
        [Column("seatcode", TypeName = "varchar(10)")]
        public string? SeatCode { get; set; }

    }
}
