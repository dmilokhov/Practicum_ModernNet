namespace EventManager.Domain.Exceptions;

public class TryBookStartedEventException : Exception
{
    public TryBookStartedEventException() { }
    public TryBookStartedEventException(string message) : base(message) { }
    public TryBookStartedEventException(string message, Exception inner) : base(message, inner) { }

}
