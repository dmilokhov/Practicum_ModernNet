using EventManager.Common.Core.Exceptions;

namespace BookingService.Domain.Exceptions;

public class TryChangeWrongBookingException(string message) : ApiException(400, message);
