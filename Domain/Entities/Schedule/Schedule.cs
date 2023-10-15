using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;

namespace Domain.Entities.Schedule
{
    public class Schedule : AuditableBaseEntity<long>
    {

        [Required]
        [Column("duration", TypeName = "int")]
        public int Duration { get; set; }

        [Required]
        [Column("description", TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }

        [Required]
        [Column("start_time", TypeName = "datetime")]
        public DateTime StartTime { get; set; }

        [Required]
        [Column("cinema_film_id", TypeName = "bigint")]
        public long CinemaFilmId { get; set; }

        [Required]
        [Column("room_id", TypeName = "bigint")]
        public long RoomId { get; set; }

        [Required]
        [Column("Price", TypeName = "int")]
        public int Price { get; set; }

    }
}
