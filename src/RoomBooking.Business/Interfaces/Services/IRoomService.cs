using Business.Utils.Domain.Utils;
using RoomBooking.Business.Utils;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.Filters;

namespace RoomBooking.Business.Interfaces.Services
{
    public interface IRoomService
    {
        Task<Room> Add(Room room);
        Task<Room> Update(Room room);
        Task<Paginator<Room>> ListRooms(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30);
        Task<Room> GetRoomById(Guid id);
        Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd);
        Task Remove(Guid Id);
        Task<ValidatorResult> Validate(Room room);

        Task<ValidatorResult> ValidateRemove(Guid Id);

    }
}
