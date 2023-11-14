using Domain.Constants.Enum;
using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ScheduleSeat
{
    public class ScheduleSeat: AuditableBaseEntity<long>
    {
        [Required]
        [Column("seat_id", TypeName ="bigint")]
        public long SeatId { get; set; }

        [Required]
        [Column("schedule_id", TypeName = "bigint")]
        public long ScheduleId { get; set; }

        [Column("reservation_time", TypeName = "datetime")]
        public DateTime? ReservationTime { get; set; }

        [Required]
        [Column("status", TypeName = "int")]
        public SeatStatus Status { get; set; }
    }
}
