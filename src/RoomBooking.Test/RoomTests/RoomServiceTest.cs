using RoomBooking.Business.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Interfaces.Repositories;
using RoomBooking.Business.Services;
using RoomBooking.Business.Interfaces.Services;
using RoomBooking.Business.Utils;

namespace Test.RoomTests
{
    public class RoomServiceTest
    {
        private Mock<IRoomRepository> _roomRepository;
        private Mock<INotificator> _notificator;
        private Mock<ILogger<RoomService>> _logger;

        private IRoomService CreateRoomServiceInstance()
        {
            _roomRepository = new Mock<IRoomRepository>();
            _notificator = new Mock<INotificator>();
            _logger = new Mock<ILogger<RoomService>>();

            return new RoomService(_roomRepository.Object, _notificator.Object, _logger.Object);
        }

        [Fact]
        public void Should_Not_Validate_Room_With_Price_Less_Than_One()
        {
            var service = CreateRoomServiceInstance();
            var room = new Room()
            {
                RoomNumber = "1",
                Price = 0
            };

            _roomRepository.Setup(x => x.GetRoomByRoomNumber(room.RoomNumber)).ReturnsAsync(null as Room);

            ValidatorResult testResult = service.Validate(room).Result;
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "The price must be greater than zero");
        }

        [Fact]
        public void Should_Not_Validate_Room_With_Same_Room_Number_And_Different_Id()
        {
            var service = CreateRoomServiceInstance();
            var roomToInsert = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Price = 1
            };
            var roomDuplicated = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Price = 10
            };
            _roomRepository.Setup(x => x.GetRoomByRoomNumber(roomToInsert.RoomNumber)).ReturnsAsync(roomDuplicated);
            ValidatorResult testResult = service.Validate(roomToInsert).Result;
            Assert.False(testResult.IsValid);
            Assert.Contains(testResult.Messages, x => x == "You can't have to room with the same number");
        }

        [Fact]
        public void Should_Validate_Room_With_Same_Room_Number_And_Same_Id()
        {
            var service = CreateRoomServiceInstance();
            var roomToInsert = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Price = 1
            };
            var roomDuplicated = new Room()
            {
                Id = roomToInsert.Id,
                RoomNumber = "1",
                Price = 10
            };
            _roomRepository.Setup(x => x.GetRoomByRoomNumber(roomToInsert.RoomNumber)).ReturnsAsync(roomDuplicated);
            ValidatorResult testResult = service.Validate(roomToInsert).Result;
            Assert.True(testResult.IsValid);
            Assert.Empty(testResult.Messages);
        }      

        [Fact]
        public void Should_Insert_Room_With_Correct_Information()
        {
            var service = CreateRoomServiceInstance();

            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Description = "Ignis aurum probat, miseria fortes viros",
                Price = 10,
                ChildrenCapacity = 2,
                AdultCapacity = 2
            };
            _roomRepository.Setup(x => x.GetRoomByRoomNumber(room.RoomNumber)).ReturnsAsync(null as Room);
            service.Add(room);
            

            _roomRepository.Verify(x => x.Add(room), Times.Once);


        }

        [Fact]
        public void Should_Update_Room_With_Correct_Information()
        {
            {
                var service = CreateRoomServiceInstance();

                var room = new Room()
                {
                    Id = Guid.NewGuid(),
                    RoomNumber = "1",
                    Description = "Ignis aurum probat, miseria fortes viros",
                    Price = 10,
                    ChildrenCapacity = 2,
                    AdultCapacity = 2
                };
                _roomRepository.Setup(x => x.GetRoomByRoomNumber(room.RoomNumber)).ReturnsAsync(null as Room);
                service.Update(room);
                
                _roomRepository.Verify(x => x.Update(room), Times.Once);


            }
        }

        [Fact]
        public void Should_Not_Remove_Room_With_Reservations()
        {
            {
                var service = CreateRoomServiceInstance();
                Guid roomId = Guid.NewGuid();
                _roomRepository.Setup(x => x.VerifyIfRoomHasAnyBookings(roomId)).ReturnsAsync(true);
                ValidatorResult testResult = service.ValidateRemove(roomId).Result;
                Assert.False(testResult.IsValid);
                Assert.Contains(testResult.Messages, x => x == "Cannot remove a room that has any bookings");
            }
        }


        [Fact]
        public void Should_Remove_Room_With_No_Reservations()
        {
            {
                var service = CreateRoomServiceInstance();
                Guid roomId = Guid.NewGuid();
                _roomRepository.Setup(x => x.VerifyIfRoomHasAnyBookings(roomId)).ReturnsAsync(false);
                service.Remove(roomId);                
                _roomRepository.Verify(x => x.Remove(roomId), Times.Once);


            }
        }


    }
}