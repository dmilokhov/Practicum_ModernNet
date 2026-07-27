namespace EventManager.Application.Interfaces.Services.Validation;

public interface ISubmitBookingValidationService
{
    public Task ValidateAsync(Guid userId, DateTime eventStartDate, CancellationToken ct = default);
}
