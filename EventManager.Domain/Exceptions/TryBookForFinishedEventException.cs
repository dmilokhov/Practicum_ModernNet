namespace EventManager.Domain.Exceptions;

public class TryBookForFinishedEventException : Exception
{
    public TryBookForFinishedEventException() { }
    public TryBookForFinishedEventException(string message) : base(message) { }
    public TryBookForFinishedEventException(string message, Exception inner) : base(message, inner) { }

}
