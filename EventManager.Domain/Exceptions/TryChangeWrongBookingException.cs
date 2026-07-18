namespace EventManager.Domain.Exceptions;

public class TryChangeWrongBookingException : Exception
{
    public TryChangeWrongBookingException() { }
    public TryChangeWrongBookingException(string message) : base(message) { }
    public TryChangeWrongBookingException(string message, Exception inner) : base(message, inner) { }

}
