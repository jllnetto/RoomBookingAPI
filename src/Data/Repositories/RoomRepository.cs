using Business.Interfaces.Repositories;
using Business.Models;
using Data.Context;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(BookingDbContext db) : base(db)
        {
        }

        public Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd)
        {
            if (id != null)
            {
                return DbSet.AnyAsync(r => r.Id == id && r.Booking.Any(res => res.BookingStarts.Date <= dateEnd.Date && dateStart.Date <= res.BookingEnds.Date));
            }
            return DbSet.AnyAsync(r => r.Booking.Any(res => res.BookingStarts.Date <= dateEnd.Date && dateStart.Date <= res.BookingEnds.Date));

        }
    }
}
