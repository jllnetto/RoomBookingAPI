using Business.Utils.Domain.Utils;
using RoomBooking.Business.Interfaces.Repositories.Base;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.Filters;

namespace RoomBooking.Business.Interfaces.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<Paginator<Room>> Search(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30);
        Task<Room> GetRoomByRoomNumber(string roomNumber);
        public Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd);
        public Task<bool> VerifyIfRoomHasAnyBookings(Guid id);
    }
}
