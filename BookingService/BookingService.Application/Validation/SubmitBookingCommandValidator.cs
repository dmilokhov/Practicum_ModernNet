using BookingService.Application.Commands;
using BookingService.Domain.Constants;
using FluentValidation;

namespace BookingService.Application.Validation;

public class SubmitBookingCommandValidator : AbstractValidator<SubmitBookingCommand>
{
    public SubmitBookingCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage(ValidationMessages.EventIdIsRequiredMsg);

        RuleFor(x => x.SeatsAmount)
            .GreaterThan(0)
            .WithMessage(ValidationMessages.SeatsAmountAboveZeroMsg);
    }
}
