using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Review
{
    public class Review: AuditableBaseEntity<long>
    {
        [Required]
        [Column("customer_id", TypeName ="bigint")]
        public long CustomerId {  get; set; }

        [Required]
        [Column("film_id", TypeName = "bigint")]
        public long FilmId { get; set; }

        [Required]
        [Column("score", TypeName = "int")]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
        public int Score { get; set; }

    }
}
