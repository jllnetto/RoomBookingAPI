using Business.Models;
using Business.Models.Validations;
using System;
using Xunit;


namespace Test.RoomTests
{
    public class RoomModelTest
    {
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
    }
}
