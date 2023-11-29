using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Responses
{
    public class AddSeatReservationResponse
    {
        public DateTime LockTime { get; set; }
        public long LockBy { get; set; }
    }
}
