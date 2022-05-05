using Business.Interfaces.Notifications;
using Business.Interfaces.Repositories;
using Business.Interfaces.Servives;
using Business.Models;
using Business.Models.Filters;
using Business.Models.Validations;
using Business.Services.Base;
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
            if (!ExecuteValidation(new RoomValidation(), room) && await Validate(room))
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
            return await _roomRepository.Search(CreateFilter(filter), currentPage, itemsPerPage);
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
            if (!ExecuteValidation(new RoomValidation(), room) && await Validate(room))
            {
                return;

            }
            await _roomRepository.Update(room);
        }

        public async Task<bool> Validate(Room room)
        {
            bool valid = true;
            if ((await _roomRepository.Search(r => r.RoomNumber.Equals(room.RoomNumber))).CountItems > 0)
            {
                Notificate("You can't have to room with the same number");
                valid = false;
            }
            return valid;
        }

        private Expression<Func<Room, bool>> CreateFilter(RoomFilter filter)
        {
            Expression<Func<Room, bool>> expression = (r => r.Id != Guid.Empty);
            if (filter is null)
            {
                return expression;
            }
            if (filter.Id is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && r.Id == filter.Id);

            }
            if (!string.IsNullOrEmpty(filter.RoomNumber))
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && r.RoomNumber.Contains(filter.RoomNumber));
            }
            if (filter.PriceBegin is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.PriceBegin >= r.Price);
            }
            if (filter.PriceFinish is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.PriceFinish <= r.Price);
            }
            if (filter.AdultCapacityBegin is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.AdultCapacityBegin >= r.AdultCapacity);
            }
            if (filter.AdultCapacityFinish is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.AdultCapacityFinish <= r.AdultCapacity);
            }
            if (filter.ChildrenCapacityBegin is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.ChildrenCapacityBegin >= r.ChildrenCapacity);
            }
            if (filter.ChildrenCapacityFinish is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.ChildrenCapacityFinish <= r.ChildrenCapacity);
            }

            return expression;
        }
    }
}
