using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Category
{
    public class Category : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name",TypeName = "nvarchar(100)")]
        public string? Name { get; set; }
    }
}
