using EventManager.Common.Core.Exceptions;

namespace BookingService.Domain.Exceptions;

public class OperationNotAllowedException(string message) : ApiException(403, message);
