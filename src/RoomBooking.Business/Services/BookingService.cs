using Business.Utils.Domain.Utils;
using Microsoft.Extensions.Logging;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Interfaces.Repositories;
using RoomBooking.Business.Interfaces.Services;
using RoomBooking.Business.Models.Validations;
using RoomBooking.Business.Services.Base;
using RoomBooking.Business.Utils;
using RoomBooking.Business.Models.Filters;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.Enums;

namespace RoomBooking.Business.Services
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IBookingRepository _bookingsRepository;
        private readonly IRoomService _roomService;
        private readonly ILogger<BaseService> _logger;

        public BookingService(IBookingRepository bookingsRepository, IRoomService roomService, INotificator notificador, ILogger<BookingService> logger) : base(notificador)
        {
            _bookingsRepository = bookingsRepository;
            _roomService = roomService;
            _logger = logger;
        }

        public async Task<Booking> Add(Booking bookings)
        {
            bookings.Total = await CalculateTotal(bookings);
            if (!ExecuteValidation(new BookingValidation(), bookings) || !(await Validate(bookings)).IsValid)
            {
                return bookings;
            }

            bookings.BookingStatus = BookingStatus.Created;
            bookings = await _bookingsRepository.Add(bookings);
            _logger.LogInformation("Booking {Id} was created", bookings.Id.ToString());
            return bookings;
        }

        public async Task CancelBooking(Guid id)
        {
            Booking canceled = await _bookingsRepository.GetById(id);

            if (!ValidateBookingCanceling(canceled).IsValid)
            {
                return;
            }

            canceled.BookingStatus = BookingStatus.Canceled;
            canceled = await _bookingsRepository.Update(canceled);
            _logger.LogInformation("Booking {Id} was canceled", canceled.Id.ToString());
        }

        public async Task CheckIn(Guid id)
        {
            Booking checkedin = await _bookingsRepository.GetById(id);
            if (!ValidateBookingCheckIn(checkedin).IsValid)
            {
                return;
            }
            checkedin.BookingStatus = BookingStatus.Executed;
            checkedin = await _bookingsRepository.Update(checkedin);
            _logger.LogInformation("Booking {Id} was checked-in", checkedin.Id.ToString());

        }

        public async Task<Booking> GetBookingById(Guid id, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await _bookingsRepository.GetByIdNoTracking(id);
            }
            return await _bookingsRepository.GetById(id);
        }

        public async Task<Paginator<Booking>> ListBookings(BookingFilter filter, int currentPage = 1, int itemsPerPage = 30)
        {
            return await _bookingsRepository.Search(filter, currentPage, itemsPerPage);
        }

        public async Task Remove(Guid id)
        {
            await _bookingsRepository.Remove(id);
            _logger.LogInformation("Booking {Id} was deleted", id.ToString());
        }

        public async Task<Booking> Update(Booking bookings)
        {
            bookings.Total = await CalculateTotal(bookings);
            if (!ExecuteValidation(new BookingValidation(), bookings) || !(await ValidateUpdate(bookings)).IsValid)
            {
                return bookings;
            }

            bookings = await _bookingsRepository.Update(bookings);
            _logger.LogInformation("Booking {Id} was updated", bookings.Id.ToString());
            return bookings;
        }

        public async Task<ValidatorResult> Validate(Booking booking)
        {
            var result = new ValidatorResult(_notificator);

            result.IsValid = true;
            if (booking.BookingStarts >= booking.BookingEnds)
            {
                result.AddMessage("The booking start date must not surpass the bookings end date");
                result.IsValid = false;
            }
            if ((booking.BookingEnds - booking.BookingStarts).TotalDays > 3)
            {
                result.AddMessage("The maximum booking time is 3 days");
                result.IsValid = false;
            }
            if (booking.BookingStarts.Date <= DateTime.Today.Date)
            {
                result.AddMessage("The booking must be made at least one day from the current date");
                result.IsValid = false;
            }
            if (booking.BookingStarts.Date >= DateTime.Today.Date.AddDays(30))
            {
                result.AddMessage("The booking cannot be set to start more than 30 days from the current date");
                result.IsValid = false;
            }
            if (booking.BookingEnds.Date > DateTime.Today.Date.AddDays(33))
            {
                result.AddMessage("The booking cannot be set to end more than 33 days from the current date");
                result.IsValid = false;
            }
            if (booking.RoomId == null || booking.RoomId == Guid.Empty)
            {
                result.AddMessage("Room is required");
                result.IsValid = false;
            }
            if (await _roomService.GetRoomById(booking.RoomId) == null)
            {
                result.AddMessage("Booked Room not found");
                result.IsValid = false;
            }
            if (!await _roomService.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds))
            {
                result.AddMessage("The requested room is not available for the requested date");
                result.IsValid = false;
            }
            return result;
        }

        public async Task<ValidatorResult> ValidateUpdate(Booking booking)
        {
            var reult = await Validate(booking);
            if (booking.BookingStatus == BookingStatus.Canceled)
            {
                reult.AddMessage("Can not update a canceled reservation");
                reult.IsValid = false;
            }
            if (booking.BookingStatus == BookingStatus.Executed)
            {
                reult.AddMessage("Can not update a checked-in reservation");
                reult.IsValid = false;
            }
            return reult;
        }

        public ValidatorResult ValidateBookingCheckIn(Booking booking)
        {
            var result = new ValidatorResult(_notificator);
            result.IsValid = true;
            if (booking.BookingStatus == BookingStatus.Executed)
            {
                result.AddMessage("This booking has already been checked-in");
                result.IsValid = false;
            }
            if (booking.BookingStatus == BookingStatus.Canceled)
            {
                result.AddMessage("Can not chenck-in because booking has been canceled");
                result.IsValid = false;
            }
            return result;
        }

        public ValidatorResult ValidateBookingCanceling(Booking booking)
        {
            var result = new ValidatorResult(_notificator);
            result.IsValid = true;
            if (booking.BookingStatus == BookingStatus.Canceled)
            {
                result.AddMessage("This booking has already been canceled");
                result.IsValid = false;
            }
            if (booking.BookingStatus == BookingStatus.Executed)
            {
                result.AddMessage("Can not cancel executed bookings");
                result.IsValid = false;
            }
            return result;
        }

        private async Task<decimal> CalculateTotal(Booking booking)
        {
            Room room = await _roomService.GetRoomById(booking.RoomId);
            decimal total = (decimal)(booking.BookingEnds.Date - booking.BookingStarts.Date).TotalDays * room.Price;
            return total;
        }

    }
}
