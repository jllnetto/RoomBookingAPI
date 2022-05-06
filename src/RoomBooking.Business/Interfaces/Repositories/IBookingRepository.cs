using Business.Utils.Domain.Utils;
using RoomBooking.Business.Interfaces.Repositories.Base;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.Filters;

namespace RoomBooking.Business.Interfaces.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Paginator<Booking>> Search(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30);
    }
}
