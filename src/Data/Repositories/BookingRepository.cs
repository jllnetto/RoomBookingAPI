using Business.Interfaces.Repositories;
using Business.Models;
using Data.Context;
using Data.Repositories.Base;

namespace Data.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(BookingDbContext db) : base(db)
        {
        }

    }
}
