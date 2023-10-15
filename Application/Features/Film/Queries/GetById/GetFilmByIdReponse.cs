using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Film.Queries.GetById
{
    public class GetFilmByIdReponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Actor { get; set; }
        public string? Director { get; set; }
        public string? Producer { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public string? Country { get; set; }
        public int LimitAge { get; set; }
        public string? Trailer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
