using Business.Models;
using Business.Models.Filters;
using Business.Utils;
using Business.Utils.Domain.Utils;

namespace Business.Interfaces.Services
{
    public interface IBookingService
    {
        public Task<Booking> Add(Booking booking);

        public Task<Paginator<Booking>> ListBookings(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30);
        Task<Booking> GetBookingById(Guid id, bool asNoTracking = false);
        public Task Remove(Guid id);

        public Task CheckIn(Guid id);
        Task CancelBooking(Guid id);

        public Task<Booking> Update(Booking booking);
        Task<ValidatorResult> Validate(Booking booking);

    }
}
