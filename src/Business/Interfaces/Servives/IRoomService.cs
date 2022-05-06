using Business.Models;
using Business.Models.Filters;
using Business.Utils;
using Business.Utils.Domain.Utils;

namespace Business.Interfaces.Servives
{
    public interface IRoomService
    {
        Task Add(Room room);
        Task Update(Room room);
        Task<Paginator<Room>> ListRooms(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30);
        Task<Room> GetRoomById(Guid id);
        Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd);
        Task Remove(Guid Id);
        Task<ValidatorResult> Validate(Room room);

    }
}
