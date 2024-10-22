using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Position
{
    public class Position : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name",TypeName = "nvarchar(100)")]
        public string? Name { get; set; }
    }
}
