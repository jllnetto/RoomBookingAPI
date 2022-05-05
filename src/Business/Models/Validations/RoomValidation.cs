using FluentValidation;

namespace Business.Models.Validations
{
    public class RoomValidation : AbstractValidator<Room>
    {
        public RoomValidation()
        {
            RuleFor(r => r.RoomNumber).NotNull().WithMessage("Room number is required").Length(1, 10).WithMessage("Room Number must be between {MinLength} and {MaxLength}");
            RuleFor(r => r.Description).NotNull().WithMessage("Room Descripton is required").Length(10, 1000).WithMessage("Room Descripton must be between {MinLength} and {MaxLength}");
            RuleFor(r => r.AdultCapacity).GreaterThan(0).WithMessage("Adult Capacity shold be larger than one");
            RuleFor(r => r.ChildrenCapacity).GreaterThan(0).WithMessage("Children Capacity shold be larger than one");

        }
    }
}
