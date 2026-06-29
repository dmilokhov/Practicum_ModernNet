namespace EventManager.Application.Interfaces.Services;

public interface IEventBookingLockProvider
{
    Task<IDisposable> AcquireAsync(Guid eventId, CancellationToken ct = default);
}
