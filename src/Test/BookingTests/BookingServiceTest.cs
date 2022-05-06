using Business.Interfaces.Notifications;
using Business.Interfaces.Repositories;
using Business.Interfaces.Servives;
using Business.Models;
using Business.Models.Enuns;
using Business.Models.Validations;
using Business.Notifications;
using Business.Services;
using Business.Utils;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;


namespace Test.BookingTests
{
    public class BookingServiceTest
    {
        private Mock<IBookingRepository> _bookingRepository;
        private Mock<IRoomService> _roomService;
        private Mock<INotificator> _notificator;

        private IBookingService CreateRoomServiceInstance()
        {
            _bookingRepository = new Mock<IBookingRepository>();
            _roomService = new Mock<IRoomService>();
            _notificator = new Mock<INotificator>();
            return new BookingService(_bookingRepository.Object, _roomService.Object, _notificator.Object);
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Start_Date_Is_larger_Than_End_Date()
        {
            var service = CreateRoomServiceInstance();
            var booking = new Booking()
            {
                Id = Guid.NewGuid(),
                BookingStarts = DateTime.Now.Date.AddDays(1),
                BookingEnds = DateTime.Now.Date,
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                Total = 10                
            };
            _roomService.Setup(x=>x.GetRoomById(booking.RoomId)).ReturnsAsync(new Room() {Id = booking.RoomId });
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId,booking.BookingStarts,booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The bookings start date must not surpass the bookings end date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Is_More_Than_Three_Say_Span()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The maximun bookings time are 3 days");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Start_Date_Equal_Or_Smaller_Than_Today()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The bookings must be made at least one day from the current date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Start_Date_More_Than_Therty_Days_From__Today()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "The bookings set to start more than 30 days from the current date");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_End_Date_More_Than_ThertyThree_Days_From__Today()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);            
            Assert.Contains(testResult.Messages, x => x == "The bookings set to end more than 33 days from the current date");        
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Room_Is_Not_Defined()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "Room is required");
        }

        [Fact]
        public void Should_Not_Validate_Booking_If_Room_Is_Not_Found()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(false);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Single(testResult.Messages);
            Assert.Contains(testResult.Messages, x => x == "Booked Room not found");
        }


        [Fact]
        public void Should_Not_Validate_Booking_If_Room_Is_Not_Available_For_The_Period()
        {
            var service = CreateRoomServiceInstance();
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
            _roomService.Setup(x => x.CheckAvailability(booking.RoomId, booking.BookingStarts, booking.BookingEnds)).ReturnsAsync(true);
            ValidatorResult testResult = service.Validate(booking).Result;
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "The requested room is not available for the requested date");
        }
    }
}
