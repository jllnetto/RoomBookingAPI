using Business.Interfaces.Repositories;
using Business.Models;
using Business.Models.Filters;
using Business.Utils.Domain.Utils;
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

        public async Task<Paginator<Room>> Search(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            var query = DbSet.AsNoTracking().Select(x => x);
            if (filter.Id is not null)
            {
                
                query = query.Where(r => r.Id == filter.Id);

            }
            if (!string.IsNullOrEmpty(filter.RoomNumber))
            {

                query = query.Where(r => r.RoomNumber.Contains(filter.RoomNumber));
            }
            if (filter.PriceBegin is not null)
            {

                query = query.Where(r => filter.PriceBegin >= r.Price);
            }
            if (filter.PriceFinish is not null)
            {

                query = query.Where(r => filter.PriceFinish <= r.Price);
            }
            if (filter.AdultCapacityBegin is not null)
            {

                query = query.Where(r => filter.AdultCapacityBegin >= r.AdultCapacity);
            }
            if (filter.AdultCapacityFinish is not null)
            {

                query = query.Where(r => filter.AdultCapacityFinish <= r.AdultCapacity);
            }
            if (filter.ChildrenCapacityBegin is not null)
            {

                query = query.Where(r => filter.ChildrenCapacityBegin >= r.ChildrenCapacity);
            }
            if (filter.ChildrenCapacityFinish is not null)
            {

                query = query.Where(r => filter.ChildrenCapacityFinish <= r.ChildrenCapacity);
            }            
            int count = await query.CountAsync();
            List<Room> data = await query
                    .OrderByDescending(c => c.CreateDate)
                    .Skip((currentPage - 1) * itemsPerPage)
                    .Take(itemsPerPage).ToListAsync();

            return new Paginator<Room>(data, count, currentPage, itemsPerPage);
        }
        public async Task<Room> GetRoomByRoomNumber(string roomNumber)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.RoomNumber == roomNumber);
        }

        public Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd)
        {
            if (id is not null)
            {
                return DbSet.AnyAsync(r => r.Id == id && r.Booking.Any(res => res.BookingStarts.Date <= dateEnd.Date && dateStart.Date <= res.BookingEnds.Date));
            }
            return DbSet.AnyAsync(r => r.Booking.Any(res => res.BookingStarts.Date <= dateEnd.Date && dateStart.Date <= res.BookingEnds.Date));

        }
    }
}
