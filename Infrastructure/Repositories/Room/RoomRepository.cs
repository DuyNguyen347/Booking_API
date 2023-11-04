using Application.Interfaces.Cinema;
using Application.Interfaces.Room;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Room
{
    public class RoomRepository : RepositoryAsync<Domain.Entities.Room.Room, long>, IRoomRepository
    {
        public RoomRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
