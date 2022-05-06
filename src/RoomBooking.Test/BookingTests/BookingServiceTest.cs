using RoomBooking.Business.Models;
using RoomBooking.Business.Models.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using RoomBooking.Business.Interfaces.Repositories;
using RoomBooking.Business.Interfaces.Services;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Services;
using RoomBooking.Business.Utils;

namespace Test.BookingTests
{
    public class BookingServiceTest
    {
        private Mock<IBookingRepository> _bookingRepository;
        private Mock<IRoomService> _roomService;
        private Mock<INotificator> _notificator;
        private Mock<ILogger<BookingService>> _logger;


        private IBookingService CreateBookingServiceInstance()
        {
            _bookingRepository = new Mock<IBookingRepository>();
            _roomService = new Mock<IRoomService>();
            _notificator = new Mock<INotificator>();
            _logger = new Mock<ILogger<BookingService>>();

            return new BookingService(_bookingRepository.Object, _roomService.Object, _notificator.Object, _logger.Object);
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Start_Date_Is_Greater_Than_End_Date()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date,
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The booking start date must not surpass the bookings end date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Is_More_Than_Three_Say_Span()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(5),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The maximum booking time is 3 days");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Start_Date_Equal_Or_Smaller_Than_Today()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date,
                BookingEnds = DateTime.Now.Date.AddDays(2),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The booking must be made at least one day from the current date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Start_Date_More_Than_Therty_Days_From__Today()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(30),
                BookingEnds = DateTime.Now.Date.AddDays(31),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The booking cannot be set to start more than 30 days from the current date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_End_Date_More_Than_ThertyThree_Days_From_Today()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(30),
                BookingEnds = DateTime.Now.Date.AddDays(34),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "The booking cannot be set to end more than 33 days from the current date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Room_Is_Not_Defined()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.Empty,
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "Room is required");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Room_Is_Not_Found()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };

            _roomService.Setup(x => x.GetRoomById(It.IsAny<Guid>())).ReturnsAsync(null as Room);
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "Booked Room not found");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Room_Is_Not_Available_For_The_Period()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            _roomService.Setup(x => x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() { Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "The requested room is not available for the requested date");
        }

        [Fact]
        public void Should_Not_Check_In_Booking_If_It_Is_Canceled()
        {
            var service = CreateBookingServiceInstance();            
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Canceled,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            ValidatorResult testResult = service.ValidateBookingCheckIn(booking);
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "Can not chenck-in because booking has been canceled");
        }

        [Fact]
        public void Should_Not_Check_In_Booking_If_It_Is_Already_Checked_In()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Executed,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            ValidatorResult testResult = service.ValidateBookingCheckIn(booking);
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "This booking has already been checked-in");
        }

        [Fact]
        public void Should_Not_Cancel_Booking_If_It_Is_Checked_In()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Executed,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            ValidatorResult testResult = service.ValidateBookingCanceling(booking);
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "Can not cancel executed bookings");
        }

        [Fact]
        public void Should_Not_Cancel_Booking_If_It_Is_Already_Canceled()
        {
            var service = CreateBookingServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date.AddDays(4),
                BookingStatus = BookingStatus.Canceled,
                RoomId = Guid.NewGuid(),
                Total = 10
            };
            ValidatorResult testResult = service.ValidateBookingCanceling(booking);
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "This booking has already been canceled");
        }

    }
}
