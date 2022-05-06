using Business.Interfaces.Notifications;
using Business.Interfaces.Repositories;
using Business.Interfaces.Servives;
using Business.Models;
using Business.Models.Enuns;
using Business.Models.Filters;
using Business.Models.Validations;
using Business.Services.Base;
using Business.Utils;
using Business.Utils.Domain.Utils;
using System.Linq.Expressions;

namespace Business.Services
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IBookingRepository _bookingsRepository;
        private readonly IRoomService _roomService;

        public BookingService(IBookingRepository bookingsRepository, IRoomService roomService, INotificator notificador) : base(notificador)
        {
            _bookingsRepository = bookingsRepository;
            _roomService = roomService;
        }

        public async Task Add(Booking bookings)
        {
            if (!ExecuteValidation(new BookingValidation(), bookings) || !(await Validate(bookings)).IsValid)
            {
                return;
            }
            await _bookingsRepository.Add(bookings);
        }

        public async Task CancelBooking(Guid id)
        {
            Booking canceled = await _bookingsRepository.GetById(id);

            if((await ValidateBookingCanceling(canceled)).IsValid)
            {
                return;
            }

            canceled.BookingStatus = BookingStatus.Canceled;
            await _bookingsRepository.Update(canceled);
        }

        public async Task CheckIn(Guid id)
        {
            Booking checkedin = await _bookingsRepository.GetById(id);
            if ((await ValidateBookingCheckIn(checkedin)).IsValid)
            {
                return;
            }
            checkedin.BookingStatus = BookingStatus.Executed;
            await _bookingsRepository.Update(checkedin);
        }

        public async Task<Booking> GetBookingById(Guid id, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await _bookingsRepository.GetByIdNoTarcking(id);
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
        }

        public async Task Update(Booking bookings)
        {
            if (!ExecuteValidation(new BookingValidation(), bookings) || !(await ValidateUpdate(bookings)).IsValid)
            {
                return;
            }
            await _bookingsRepository.Update(bookings);
        }

        public async Task<ValidatorResult> Validate(Booking booking)
        {
            var result = new ValidatorResult(_notificator);

            result.IsValid = true;
            if (booking.BookingStarts >= booking.BookingEnds)
            {
                result.AddMessage("The bookings start date must not surpass the bookings end date");
                result.IsValid = false;
            }
            if ((booking.BookingEnds - booking.BookingStarts).TotalDays > 3)
            {
                result.AddMessage("The maximun bookings time are 3 days");
                result.IsValid = false;
            }
            if (booking.BookingStarts.Date <= DateTime.Today.Date)
            {
                result.AddMessage("The bookings must be made at least one day from the current date");
                result.IsValid = false;
            }
            if (booking.BookingStarts.Date >= DateTime.Today.Date.AddDays(30))
            {
                result.AddMessage("The bookings set to start more than 30 days from the current date");
                result.IsValid = false;
            }
            if (booking.BookingEnds.Date > DateTime.Today.Date.AddDays(33))
            {
                result.AddMessage("The bookings set to end more than 33 days from the current date");
                result.IsValid = false;
            }
            if (booking.RoomId == null || booking.RoomId == Guid.Empty)
            {
                result.AddMessage("Room is required");
                result.IsValid = false;
            }
            if ((await _roomService.GetRoomById(booking.RoomId)) ==null)
            {
                result.AddMessage("Booked Room not found");
                result.IsValid = false;
            }
            if (await _roomService.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds))
            {
                result.AddMessage("The requested room is not available for the requested date");
                result.IsValid = false;
            }
            return result;
        }

        public async Task<ValidatorResult> ValidateUpdate(Booking booking)
        {
            var reult = await Validate(booking);
            if(booking.BookingStatus == BookingStatus.Canceled)
            {
                reult.AddMessage("Can not update a canceled reservation");
                reult.IsValid = false;
            }
            if(booking.BookingStatus == BookingStatus.Executed)
            {
                reult.AddMessage("Can not update a checked-in reservation");
                reult.IsValid=false;
            }
            return reult;
        }

        public async Task<ValidatorResult> ValidateBookingCheckIn(Booking booking)
        {
            var result = new ValidatorResult(_notificator);
            result.IsValid = true;
            if(booking.BookingStatus == BookingStatus.Executed)
            {
                result.AddMessage("This booking has already been checked-in");
                result.IsValid=false;
            }
            if (booking.BookingStatus == BookingStatus.Canceled)
            {
                result.AddMessage("Can not chenck-in because booking has been canceled");
                result.IsValid = false;
            }
            return result;
        }

        public async Task<ValidatorResult> ValidateBookingCanceling(Booking booking)
        {
            var result = new ValidatorResult(_notificator);
            result.IsValid = true;
            if(booking.BookingStatus == BookingStatus.Canceled)
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

    }
}
