using Business.Interfaces.Notifications;
using Business.Interfaces.Repositories;
using Business.Interfaces.Services;
using Business.Models;
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

namespace Test.RoomTests
{
    public class RoomServiceTest
    {
        private Mock<IRoomRepository> _roomRepository;
        private Mock<INotificator> _notificator;

        private IRoomService CreateRoomServiceInstance()
        {
            _roomRepository = new Mock<IRoomRepository>();
            _notificator = new Mock<INotificator>();
            return new RoomService(_roomRepository.Object, _notificator.Object);
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
        public void Should_Not_Validate_Room_With_No_Room_Number()
        {
            var validation = new RoomValidation();
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = null,
                Description = "Ignis aurum probat, miseria fortes viros",
                Price = 10,
                ChildrenCapacity = 2,
                AdultCapacity = 2
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Room number is required");

        }

        [Fact]
        public void Should_Not_Validate_Room_With_Empty_Room_Number()
        {
            var validation = new RoomValidation();
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "",
                Description = "Ignis aurum probat, miseria fortes viros",
                Price = 10,
                ChildrenCapacity = 2,
                AdultCapacity = 2
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Room Number must be between 1 and 10");

        }

        [Fact]
        public void Should_Not_Validate_Room_With_No_Description()
        {
            var validation = new RoomValidation();
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Description = null,
                Price = 10,
                ChildrenCapacity = 2,
                AdultCapacity = 2
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Room Descripton is required");
        }

        [Fact]
        public void Should_Not_Validate_Room_With_Empty_Description()
        {
            var validation = new RoomValidation();

            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Description = "",
                Price = 10,
                ChildrenCapacity = 2,
                AdultCapacity = 2
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Room Descripton must be between 10 and 1000");
        }

        [Fact]
        public void Should_Not_Validate_Room_With_Price_Less_Them_One()
        {
            var validation = new RoomValidation();
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Description = "Ignis aurum probat, miseria fortes viros",
                Price = 0,
                ChildrenCapacity = 2,
                AdultCapacity = 2
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Price should be greater than zero");
        }

        [Fact]
        public void Should_Not_Validate_Room_With_Adult_Capacity_Less_Them_One()
        {
            var validation = new RoomValidation();
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                RoomNumber = "1",
                Description = "Ignis aurum probat, miseria fortes viros",
                Price = 10,
                ChildrenCapacity = 2,
                AdultCapacity = 0
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Adult Capacity should be greater than zero");
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

            service.Add(room);
            _roomRepository.Setup(x => x.GetRoomByRoomNumber(room.RoomNumber)).ReturnsAsync(null as Room);

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

                service.Update(room);
                _roomRepository.Setup(x => x.GetRoomByRoomNumber(room.RoomNumber)).ReturnsAsync(null as Room);
                _roomRepository.Verify(x => x.Update(room), Times.Once);


            }
        }
    }
}