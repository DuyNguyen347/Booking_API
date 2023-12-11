using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.CinemaImage
{
    public class CinemaImage :AuditableBaseEntity<long>
    {
        [Required]
        [Column("cinema_id",TypeName ="bigint")]
        public long CinemaId { get; set; }
        [Required]
        [Column("name_file",TypeName ="varchar(MAX)")]
        public string? NameFile { get; set; }
    }
}
