using API.Controllers;
using AutoMapper;
using Business.Interfaces.Notifications;
using Business.Interfaces.Services;
using Business.Models;
using Business.Models.DTOs.Input;
using Business.Models.DTOs.Output;
using Business.Models.Filters;
using Business.Utils.Domain.Utils;
using Microsoft.AspNetCore.Mvc;


namespace API.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/{version:apiVersion}/bookings")]
    public class BookingController : MainController
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        public BookingController(IBookingService bookingService, IMapper mapper, INotificator notificator) : base(notificator)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpPost("GetAll")]
        public async Task<Paginator<BookingOutputDTO>> GetAll(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            return _mapper.Map<Paginator<BookingOutputDTO>>(await _bookingService.ListBookings(filter, currentPage, itemsPerPage));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingOutputDTO>> GetById(Guid id)
        {
            BookingOutputDTO booking = _mapper.Map<BookingOutputDTO>(await _bookingService.GetBookingById(id));
            if (booking is null)
            {
                return NotFound();
            }
            return CustomResponse(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingOutputDTO>> Add(BookingInputDTO booking)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            return CustomResponse(_mapper.Map<BookingOutputDTO>(await _bookingService.Add(_mapper.Map<Booking>(booking)))); ;
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BookingOutputDTO>> Update(Guid id, BookingInputDTO booking)
        {

            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            BookingInputDTO bookingUpdate = _mapper.Map<BookingInputDTO>(await _bookingService.GetBookingById(id));

            if (bookingUpdate is null)
            {
                return NotFound();
            }

            bookingUpdate.BookingStarts = booking.BookingStarts;
            bookingUpdate.BookingEnds = booking.BookingEnds;
            bookingUpdate.RoomId = booking.RoomId;


            return CustomResponse(_mapper.Map<BookingOutputDTO>(await _bookingService.Update(_mapper.Map<Booking>(bookingUpdate))));
        }

        [HttpPut("CancelBooking")]

        public async Task<ActionResult> CancelBooking(Guid id)
        {
            if (await _bookingService.GetBookingById(id, true) is null)
            {
                return NotFound();
            }
            await _bookingService.CancelBooking(id);
            return CustomResponse();
        }

        [HttpPut("CheckIn")]
        public async Task<ActionResult> CheckIn(Guid id)
        {
            if (await _bookingService.GetBookingById(id, true) is null)
            {
                return NotFound();
            }
            await _bookingService.CheckIn(id);
            return CustomResponse();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<BookingOutputDTO>> Delete(Guid id)
        {
            BookingOutputDTO bookingRemove = _mapper.Map<BookingOutputDTO>(await _bookingService.GetBookingById(id));
            if (bookingRemove is null)
            {
                return NotFound();
            }
            await _bookingService.Remove(id);
            return CustomResponse(bookingRemove);
        }
    }
}
