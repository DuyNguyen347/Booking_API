﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Constants.Enum;
using Domain.Contracts;

namespace Domain.Entities.Seat
{
    public class Seat : AuditableBaseEntity<long>
    {

        [Required]
        [Column("room_id", TypeName = "bigint")]
        public long RoomId { get; set; }


        [Required]
        [Column("number", TypeName = "int")]
        public int NumberSeat { get; set; }

        [Required]
        [Column("status", TypeName = "int")]
        public RoomStatus Status { get; set; }

    }
}