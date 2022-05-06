using Business.Interfaces.Repositories.Base;
using Business.Models;
using Business.Models.Filters;
using Business.Utils.Domain.Utils;

namespace Business.Interfaces.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<Paginator<Room>> Search(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30);
        Task<Room> GetRoomByRoomNumber(string roomNumber);
        public Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd);
    }
}
