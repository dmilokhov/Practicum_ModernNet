namespace EventManager.Domain.Exceptions;

public class BookingLimitOverflowException : Exception
{
    public BookingLimitOverflowException() { }
    public BookingLimitOverflowException(string message) : base(message) { }
    public BookingLimitOverflowException(string message, Exception inner) : base(message, inner) { }

}
