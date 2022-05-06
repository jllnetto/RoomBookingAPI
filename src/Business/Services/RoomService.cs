using Business.Interfaces.Notifications;
using Business.Interfaces.Repositories;
using Business.Interfaces.Servives;
using Business.Models;
using Business.Models.Filters;
using Business.Models.Validations;
using Business.Services.Base;
using Business.Utils;
using Business.Utils.Domain.Utils;
using System.Linq.Expressions;

namespace Business.Services
{
    public class RoomService : BaseService, IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        public RoomService(IRoomRepository roomRepository, INotificator notificador) : base(notificador)
        {
            _roomRepository = roomRepository;
        }

        public async Task Add(Room room)
        {
            if (!ExecuteValidation(new RoomValidation(), room) || !(await Validate(room)).IsValid)
            {
                return;
            }

            await _roomRepository.Add(room);
        }

        public async Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd)
        {
            return await _roomRepository.CheckAvailability(id, dateStart, dateEnd);
        }

        public async Task<Paginator<Room>> ListRooms(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            return await _roomRepository.Search(filter, currentPage, itemsPerPage);
        }

        public async Task<Room> GetRoomById(Guid id)
        {
            return await _roomRepository.GetById(id);
        }

        public async Task Remove(Guid Id)
        {
            await _roomRepository.Remove(Id);
        }

        public async Task Update(Room room)
        {
            if (!ExecuteValidation(new RoomValidation(), room) || !(await Validate(room)).IsValid)
            {
                return;
            }

            await _roomRepository.Update(room);
        }

        public async Task<ValidatorResult> Validate(Room room)
        {
            var result = new ValidatorResult(_notificator);            
            result.IsValid = true;
            if ((await _roomRepository.GetRoomByRoomNumber(room.RoomNumber)) is not null && (await _roomRepository.GetRoomByRoomNumber(room.RoomNumber)).Id != room.Id)
            {
                result.AddMessage("You can't have to room with the same number");
                result.IsValid = false;
            }
            if(room.Price <= 0)
            {
                result.AddMessage("The price must be greater than zero");
                result.IsValid = false;
            }

            return result;
        }
        
    }
}
