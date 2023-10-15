using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Cinema
{
    public class Cinema : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name", TypeName = "nvarchar(100)")]
        public string? Name { get; set; }

        [Column("description", TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }

        [Column("city",TypeName = "nvarchar(50)")]
        public string? City { get; set; }


    }
}
