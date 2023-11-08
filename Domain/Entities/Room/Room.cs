using Domain.Constants.Enum;
using Domain.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        [Column("number_row",TypeName = "int")]
        public int NumberRow { get; set; }


        [Required]
        [Column("number_column", TypeName = "int")]
        public int NumberColumn { get; set; }

    }
}
