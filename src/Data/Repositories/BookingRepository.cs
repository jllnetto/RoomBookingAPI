using Business.Interfaces.Repositories;
using Business.Models;
using Business.Models.Filters;
using Business.Utils.Domain.Utils;
using Data.Context;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(BookingDbContext db) : base(db)
        {
        }

        public async Task<Paginator<Booking>> Search(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            var query = DbSet.AsNoTracking().Select(x => x);

            if(filter is not null)
            {
                if (filter.Id is not null)
                {
                    query = query.Where(r => r.Id == filter.Id);
                }
                if (!string.IsNullOrEmpty(filter.RoomNumber))
                {
                    query = query.Where(r => r.Room.RoomNumber.Contains(filter.RoomNumber));
                }
                if (filter.BookingStatus is not null)
                {
                    query = query.Where(r => filter.BookingStatus == (int)r.BookingStatus);
                }
                if (filter.StartDateBegin is not null)
                {
                    query = query.Where(r => filter.StartDateBegin.Value.Date >= r.BookingStarts.Date);
                }
                if (filter.StartDateFinish is not null)
                {
                    query = query.Where(r => filter.StartDateFinish.Value.Date <= r.BookingStarts.Date);
                }
                if (filter.EndDateBegin is not null)
                {
                    query = query.Where(r => filter.EndDateBegin.Value.Date >= r.BookingEnds.Date);
                }
                if (filter.EndDateFinish is not null)
                {
                    query = query.Where(r => filter.EndDateFinish.Value.Date <= r.BookingEnds.Date);
                }
                if (filter.RoomId is not null)
                {
                    query = query.Where(r => filter.RoomId.Value == r.RoomId);
                }
            }

            int count = await query.CountAsync();
            List<Booking> data = await query
                    .OrderByDescending(c => c.CreateDate)
                    .Skip((currentPage - 1) * itemsPerPage)
                    .Take(itemsPerPage).ToListAsync();

            return new Paginator<Booking>(data, count, currentPage, itemsPerPage);

        }

    }
}
