using AutoMapper;
using Business.Utils.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Controllers;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Interfaces.Services;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.DTOs.Input;
using RoomBooking.Business.Models.DTOs.Output;
using RoomBooking.Business.Models.Filters;

namespace RoomBooking.API.V1.Controllers
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

        [HttpPost("GetAll")]
        public async Task<Paginator<RoomOutputDTO>> GetAll(RoomFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            return _mapper.Map<Paginator<RoomOutputDTO>>(await _roomService.ListRooms(filter, currentPage, itemsPerPage));
        }

        [HttpGet("CheckAvailability")]
        public async Task<ActionResult<bool>> CheckAvailability(Guid? id, DateTime dateBegin, DateTime dateEnd)
        {
            return CustomResponse(await _roomService.CheckAvailability(id, dateBegin, dateEnd));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoomOutputDTO>> GetById(Guid id)
        {
            RoomOutputDTO room = _mapper.Map<RoomOutputDTO>(await _roomService.GetRoomById(id));
            if (room is null)
            {
                return NotFound();
            }
            return CustomResponse(room);
        }

        [HttpPost]
        public async Task<ActionResult<RoomOutputDTO>> Add(RoomInputDTO room)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            return CustomResponse(_mapper.Map<RoomOutputDTO>(await _roomService.Add(_mapper.Map<Room>(room))));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<RoomOutputDTO>> Update(Guid id, RoomInputDTO room)
        {

            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            RoomInputDTO roomUpdate = _mapper.Map<RoomInputDTO>(await _roomService.GetRoomById(id));

            if (roomUpdate is null)
            {
                return NotFound();
            }

            roomUpdate.RoomNumber = room.RoomNumber;
            roomUpdate.Description = room.Description;
            roomUpdate.Price = room.Price;
            roomUpdate.AdultCapacity = room.AdultCapacity;
            roomUpdate.ChildrenCapacity = room.ChildrenCapacity;

            var roomMapped = _mapper.Map<Room>(roomUpdate);
            roomMapped.Id = id;

            return CustomResponse(_mapper.Map<RoomOutputDTO>(await _roomService.Update(roomMapped)));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<RoomOutputDTO>> Delete(Guid id)
        {
            RoomOutputDTO roomRemove = _mapper.Map<RoomOutputDTO>(await _roomService.GetRoomById(id));
            if (roomRemove is null)
            {
                return NotFound();
            }

            await _roomService.Remove(id);

            return CustomResponse(roomRemove);
        }

    }
}
