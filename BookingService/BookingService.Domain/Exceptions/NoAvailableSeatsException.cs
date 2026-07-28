using EventManager.Common.Core.Exceptions;

namespace BookingService.Domain.Exceptions;

public class NoAvailableSeatsException(string message) : ApiException(409, message);
