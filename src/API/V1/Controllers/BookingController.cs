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

        [HttpGet]
        public async Task<Paginator<BookingDTO>> GetAll(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            return _mapper.Map<Paginator<BookingDTO>>(await _bookingService.ListBookings(filter, currentPage, itemsPerPage));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingDTO>> GetById(Guid id)
        {
            BookingDTO booking = _mapper.Map<BookingDTO>(await _bookingService.GetBookingById(id));
            if (booking is null)
            {
                return NotFound();
            }
            return CustomResponse(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDTO>> Add(BookingDTO booking)
        {
            if (!ModelState.IsValid)
            {
                CustomResponse(ModelState);
            }
            await _bookingService.Add(_mapper.Map<Booking>(booking));
            return CustomResponse(booking);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BookingDTO>> Update(Guid id, BookingDTO booking)
        {
            if (id != booking.Id)
            {
                NotificateError("The Id informed are not the same");
                return CustomResponse();
            }
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            BookingDTO bookingUpdate = _mapper.Map<BookingDTO>(await _bookingService.GetBookingById(id));

            if (bookingUpdate is null)
            {
                return NotFound();
            }

            bookingUpdate.BookingStarts = booking.BookingStarts;
            bookingUpdate.BookingEnds = booking.BookingEnds;
            bookingUpdate.RoomId = booking.RoomId;
            await _bookingService.Update(_mapper.Map<Booking>(bookingUpdate));

            BookingDTO bookingDTO = _mapper.Map<BookingDTO>(_bookingService.GetBookingById(id));

            return CustomResponse(bookingDTO);
        }


        [HttpPut("CancelBooking")]

        public async Task<ActionResult> CancelBooking(Guid id)
        {
            if (await _bookingService.GetBookingById(id) is null)
            {
                return NotFound();
            }
            await _bookingService.CancelBooking(id);
            return CustomResponse();
        }

        [HttpPut("CheckIn")]
        public async Task<ActionResult> CheckIn(Guid id)
        {
            if (await _bookingService.GetBookingById(id) is null)
            {
                return NotFound();
            }
            await _bookingService.CheckIn(id);
            return CustomResponse();
        }



        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<BookingDTO>> Delete(Guid id)
        {
            BookingDTO bookingRemove = _mapper.Map<BookingDTO>(await _bookingService.GetBookingById(id));
            if (bookingRemove is null)
            {
                return NotFound();
            }
            await _bookingService.Remove(id);
            return CustomResponse(bookingRemove);
        }
    }
}
