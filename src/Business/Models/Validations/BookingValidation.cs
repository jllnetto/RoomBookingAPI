using FluentValidation;

namespace Business.Models.Validations
{
    public class BookingValidation : AbstractValidator<Booking>
    {
        public BookingValidation()
        {
            RuleFor(x => x.Total).GreaterThan(0).WithMessage("Total should be greater than zero");
        }
    }
}
