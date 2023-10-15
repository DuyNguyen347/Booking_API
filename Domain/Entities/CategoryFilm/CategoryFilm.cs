using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CategoryFilm
{
    public class CategoryFilm : AuditableBaseEntity<long>
    {
        [Required]
        [Column("category_id",TypeName = "bigint")]
        public long CategoryId { get; set; }

        [Required]
        [Column("film_id",TypeName = "bigint")]
        public long FilmId { get; set; }

        [Column("describe",TypeName = "nvarchar(MAX)")]
        public string? Describe {get; set; }
    }
}
