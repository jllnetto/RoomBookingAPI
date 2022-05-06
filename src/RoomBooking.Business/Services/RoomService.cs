using Business.Utils.Domain.Utils;
using Microsoft.Extensions.Logging;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Interfaces.Repositories;
using RoomBooking.Business.Interfaces.Services;
using RoomBooking.Business.Models.Validations;
using RoomBooking.Business.Services.Base;
using RoomBooking.Business.Utils;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.Filters;

namespace RoomBooking.Business.Services
{
    public class RoomService : BaseService, IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository, INotificator notificador, ILogger<RoomService> logger) : base(notificador)
        {
            _roomRepository = roomRepository;
            _logger = logger;
        }

        public async Task<Room> Add(Room room)
        {
            if (!ExecuteValidation(new RoomValidation(), room) || !(await Validate(room)).IsValid)
            {
                return room;
            }

            room = await _roomRepository.Add(room);
            _logger.LogInformation("Room {Id} was created", room.Id.ToString());
            return room;
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
            if (!(await ValidateRemove(Id)).IsValid)
            {
                return;
            }

            await _roomRepository.Remove(Id);
            _logger.LogInformation("Room {Id} was deleted", Id.ToString());
        }

        public async Task<Room> Update(Room room)
        {
            if (!ExecuteValidation(new RoomValidation(), room) || !(await Validate(room)).IsValid)
            {
                return room;
            }

            room = await _roomRepository.Update(room);
            _logger.LogInformation("Room {Id} was updated", room.Id.ToString());
            return room;
        }

        public async Task<ValidatorResult> Validate(Room room)
        {
            var result = new ValidatorResult(_notificator);
            result.IsValid = true;
            if (await _roomRepository.GetRoomByRoomNumber(room.RoomNumber) is not null && (await _roomRepository.GetRoomByRoomNumber(room.RoomNumber)).Id != room.Id)
            {
                result.AddMessage("You can't have to room with the same number");
                result.IsValid = false;
            }
            if (room.Price <= 0)
            {
                result.AddMessage("The price must be greater than zero");
                result.IsValid = false;
            }

            return result;
        }

        public async Task<ValidatorResult> ValidateRemove(Guid Id)
        {
            var result = new ValidatorResult(_notificator);
            result.IsValid = true;
            if (await _roomRepository.VerifyIfRoomHasAnyBookings(Id))
            {
                result.AddMessage("Cannot remove a room that has any bookings");
                result.IsValid = false;
            }

            return result;
        }

    }
}
