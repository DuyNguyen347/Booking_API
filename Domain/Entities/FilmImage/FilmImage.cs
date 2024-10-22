using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.FilmImage
{
    public class FilmImage : AuditableBaseEntity<long>
    {
        [Required]
        [Column("films_id", TypeName = "bigint")]
        public long FilmId { get; set; }

        [Required]
        [Column("name_file", TypeName = "nvarchar(MAX)")]
        public string? NameFile { get; set; }

    }
}
