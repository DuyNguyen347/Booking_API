using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Poster
{
    public class Poster : AuditableBaseEntity<long>
    {
        [Required]
        [Column("path-image",TypeName="nvarchar(MAX)")]
        public string? PathImage { get; set; }

        [Column("link-url",TypeName = "varchar(MAX)")]
        public string? LinkUrl { get; set; }
    }
}
