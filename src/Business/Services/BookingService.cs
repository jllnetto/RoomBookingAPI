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
            if (!ExecuteValidation(new BookingValidation(), bookings) && await Validate(bookings))
            {
                return;
            }
            await _bookingsRepository.Add(bookings);
        }

        public async Task CancelBooking(Guid id)
        {
            Booking canceled = await _bookingsRepository.GetById(id);

            canceled.BookingStatus = Models.Enuns.BookingStatus.Canceled;
            await Update(canceled);

        }

        public async Task CheckIn(Guid id)
        {
            Booking checkedin = await _bookingsRepository.GetById(id);

            checkedin.BookingStatus = Models.Enuns.BookingStatus.Executed;
            await Update(checkedin);
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
            return await _bookingsRepository.Search(CreateFilter(filter), currentPage, itemsPerPage);
        }

        public async Task Remove(Guid id)
        {
            await _bookingsRepository.Remove(id);
        }

        public async Task Update(Booking bookings)
        {
            if (!ExecuteValidation(new BookingValidation(), bookings) && await Validate(bookings))
            {
                return;
            }
            await _bookingsRepository.Update(bookings);
        }

        public async Task<bool> Validate(Booking bookings)
        {
            bool valid = true;
            if (bookings.BookingStarts <= bookings.BookingEnds)
            {
                Notificate("The bookings start date must not surpass the bookings end date");
                valid = false;
            }
            if ((bookings.BookingStarts - bookings.BookingEnds).TotalDays > 3)
            {
                Notificate("The maximun bookings time are 3 days");
                valid = false;
            }
            if (bookings.BookingStarts.Date <= DateTime.Today.Date)
            {
                Notificate("The bookings must be made at least one day from the current date");
                valid = false;
            }
            if (bookings.BookingStarts.Date > DateTime.Today.Date.AddDays(30))
            {
                Notificate("The bookings set to start more than 30 days from the current date");
                valid = false;
            }
            if (bookings.BookingEnds.Date > DateTime.Today.Date.AddDays(30))
            {
                Notificate("The bookings set to end more than 30 days from the current date");
                valid = false;
            }
            if (await _roomService.CheckAvailability(bookings.RoomId, bookings.BookingStarts, bookings.BookingEnds))
            {
                Notificate("The requeste room is not available for the requested date");
                valid = false;
            }
            return valid;
        }
        private Expression<Func<Booking, bool>> CreateFilter(BookingFilter filter)
        {
            Expression<Func<Booking, bool>> expression = (r => r.Id != Guid.Empty);
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
                expression = (r => compiled(r) && r.Room.RoomNumber.Contains(filter.RoomNumber));

            }
            if (filter.BookingStatus is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.BookingStatus == (int)r.BookingStatus);
            }
            if (filter.StartDateBegin is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.StartDateBegin.Value.Date >= r.BookingStarts.Date);
            }
            if (filter.StartDateFinish is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.StartDateFinish.Value.Date <= r.BookingStarts.Date);
            }
            if (filter.EndDateBegin is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.EndDateBegin.Value.Date >= r.BookingEnds.Date);
            }
            if (filter.EndDateFinish is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.EndDateFinish.Value.Date <= r.BookingEnds.Date);
            }
            if (filter.RoomId is not null)
            {
                var compiled = expression.Compile();
                expression = (r => compiled(r) && filter.RoomId.Value == r.RoomId);
            }

            return expression;
        }

    }
}
