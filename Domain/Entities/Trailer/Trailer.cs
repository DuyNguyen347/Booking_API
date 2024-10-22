using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;

namespace Domain.Entities.Trailer
{
    public class Trailer : AuditableBaseEntity<long>
    {
        [Required]
        [Column("film_id", TypeName = "bigint")]
        public long FilmId { get; set; }

        [Required]
        [Column("name_file", TypeName = "nvarchar(MAX)")]
        public string? NameFile { get; set; }

    }
}
