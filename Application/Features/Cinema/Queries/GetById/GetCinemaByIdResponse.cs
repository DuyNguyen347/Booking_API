using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Cinema.Queries.GetById
{
    public class GetCinemaByIdResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? Hotline { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? Address { get; set; }
        public List<string> listImage { get; set;  }

    }
}
