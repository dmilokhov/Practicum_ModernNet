using EventManager.Common.Core.Exceptions;

namespace BookingService.Domain.Exceptions;

public class TryBookStartedEventException(string message) : ApiException(400, message);
