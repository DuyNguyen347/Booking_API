using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;

namespace Domain.Entities.CinemaFilm
{
    public class CinemaFilm : AuditableBaseEntity<long>
    {
        [Required]
        [Column("cinema_id", TypeName = "bigint")]
        public long CinemaId { get; set; }

        [Required]
        [Column("film_id", TypeName = "bigint")]
        public long FilmId { get; set; }
    }
}
