using EventManager.Common.Core.Exceptions;

namespace BookingService.Domain.Exceptions;

public class BookingLimitOverflowException(string message) : ApiException(409, message);
