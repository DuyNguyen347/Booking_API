using Domain.Constants.Enum;
using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Room
{
    public class Room : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name",TypeName = "nvarchar(50)")]
        public string? Name { get; set; }

        [Required]
        [Column("number_seat",TypeName = "int")]
        public int NumberSeat { get; set; }


        [Required]
        [Column("status", TypeName = "int")]
        public SeatStatus Status { get; set; }


        [Required]
        [Column("cinema_id", TypeName = "bigint")]
        public long CinemaId { get; set; }

    }
}
