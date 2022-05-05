using API.Controllers;
using AutoMapper;
using Business.Interfaces.Notifications;
using Business.Interfaces.Servives;
using Business.Models;
using Business.Models.DTOs;
using Business.Models.Filters;
using Business.Utils.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace API.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/{version:apiVersion}/rooms")]
    public class RoomController : MainController
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomController(IRoomService roomService, IMapper mapper, INotificator notificator) : base(notificator)
        {
            _roomService = roomService;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<Paginator<RoomDTO>> GetAll(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            return _mapper.Map<Paginator<RoomDTO>>(await _roomService.ListRooms(filter, currentPage, itemsPerPage));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoomDTO>> GetById(Guid id)
        {
            RoomDTO room = _mapper.Map<RoomDTO>(await _roomService.GetRoomById(id));
            if (room is null)
            {
                return NotFound();
            }
            return CustomResponse(room);
        }

        [HttpPost]
        public async Task<ActionResult<RoomDTO>> Add(RoomDTO room)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }
            await _roomService.Add(_mapper.Map<Room>(room));
            return CustomResponse(room);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<RoomDTO>> Update(Guid id, RoomDTO room)
        {
            if (id != room.Id)
            {
                NotificateError("The Id informed are not the same");
                return CustomResponse();
            }
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            RoomDTO roomUpdate = _mapper.Map<RoomDTO>(await _roomService.GetRoomById(id));

            if (roomUpdate is null)
            {
                return NotFound();
            }

            roomUpdate.RoomNumber = room.RoomNumber;
            roomUpdate.Description = room.Description;
            roomUpdate.Price = room.Price;
            roomUpdate.AdultCapacity = room.AdultCapacity;
            roomUpdate.ChildrenCapacity = room.ChildrenCapacity;
            await _roomService.Update(_mapper.Map<Room>(roomUpdate));

            RoomDTO roomUpdated = _mapper.Map<RoomDTO>(_roomService.GetRoomById(id));

            return CustomResponse(roomUpdated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<RoomDTO>> Delete(Guid id)
        {
            RoomDTO roomRemove = _mapper.Map<RoomDTO>(await _roomService.GetRoomById(id));
            if (roomRemove is null)
            {
                return NotFound();
            }

            await _roomService.Remove(id);

            return CustomResponse(roomRemove);
        }



    }
}
