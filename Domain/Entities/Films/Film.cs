using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Films
{
    public class Film : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name", TypeName = "nvarchar(200)")]
        public string? Name { get; set; }

        [Required]
        [Column("actor",TypeName = "nvarchar(500)")]
        public string? Actor { get; set; }

        [Required]
        [Column("director", TypeName = "nvarchar(500)")]
        public string? Director { get; set;}

        [Required]
        [Column("producer", TypeName = "nvarchar(200)")]
        public string? Producer { get; set; }

        [Required]
        [Column("duration", TypeName = "int")]
        public int Duration { get; set; }

        [Required]
        [Column("describe",TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [Required]
        [Column("year", TypeName ="int")]
        public int Year { get; set; }

        [Required]
        [Column("country_id",TypeName = "nvarchar(100)")]
        public string? Country { get; set; }

        [Required]
        [Column("limit_age",TypeName = "int")]
        public int LimitAge { get; set; }

        [Required]
        [Column("trailer",TypeName = "nvarchar(MAX)")]
        public string? Trailer { get; set; }

        [Required]
        [Column("start_date", TypeName = "datetime")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("end_date", TypeName = "datetime")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("enable", TypeName = "bit")]
        public bool Enable { get; set; }

        [Required]
        [Column("poster", TypeName = "varchar(200)")]
        public string? Poster { get; set; }

    }
}
