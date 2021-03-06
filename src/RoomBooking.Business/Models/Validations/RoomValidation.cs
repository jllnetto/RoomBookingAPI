using FluentValidation;
using RoomBooking.Business.Models;

namespace RoomBooking.Business.Models.Validations
{
    public class RoomValidation : AbstractValidator<Room>
    {
        public RoomValidation()
        {
            RuleFor(r => r.RoomNumber).NotNull().WithMessage("Room number is required").Length(1, 10).WithMessage("Room Number must be between {MinLength} and {MaxLength}");
            RuleFor(r => r.Description).NotNull().WithMessage("Room Descripton is required").Length(10, 1000).WithMessage("Room Descripton must be between {MinLength} and {MaxLength}");
            RuleFor(r => r.Price).GreaterThan(0).WithMessage("Price should be greater than zero");
            RuleFor(r => r.AdultCapacity).GreaterThan(0).WithMessage("Adult Capacity should be greater than zero");
            RuleFor(r => r.ChildrenCapacity).GreaterThanOrEqualTo(0).WithMessage("Children Capacity should be greater than zero");
        }
    }
}
