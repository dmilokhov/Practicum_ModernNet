using EventManager.Application.Commands;

namespace EventManager.Application.Interfaces.Services.Validation;

public interface ISubmitBookingValidator
{
    public Task ValidateAsync(SubmitBookingCommand command, CancellationToken ct = default);
}
