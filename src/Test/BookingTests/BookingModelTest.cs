using Business.Models;
using Business.Models.Enums;
using Business.Models.Validations;
using System;
using Xunit;

namespace Test.BookingTests
{
    public class BookingModelTest
    {
        [Fact]
        public void Should_Not_Validate_Booking_With_Total_Less_Them_One()
        {
            var validation = new BookingValidation();
            var room = new Booking()
            {
                Id = Guid.NewGuid(),
                Total = 0,
                BookingStarts = DateTime.Now.AddDays(1),
                BookingEnds = DateTime.Now.AddDays(4),
                BookingStatus = BookingStatus.Created,
                RoomId = Guid.NewGuid(),
                
            };
            var validator = validation.Validate(room);
            Assert.False(validator.IsValid);
            Assert.Equal(1, validator.Errors.Count);
            Assert.Contains(validator.Errors, x => x.ErrorMessage == "Total should be greater than zero");
        }
        
    }
}
