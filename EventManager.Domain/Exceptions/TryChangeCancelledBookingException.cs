namespace EventManager.Domain.Exceptions;

public class TryChangeCancelledBookingException : Exception
{
    public TryChangeCancelledBookingException() { }
    public TryChangeCancelledBookingException(string message) : base(message) { }
    public TryChangeCancelledBookingException(string message, Exception inner) : base(message, inner) { }

}
