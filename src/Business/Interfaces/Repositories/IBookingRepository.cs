using Business.Interfaces.Repositories.Base;
using Business.Models;
using Business.Models.Filters;
using Business.Utils.Domain.Utils;

namespace Business.Interfaces.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Paginator<Booking>> Search(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30);
    }
}
