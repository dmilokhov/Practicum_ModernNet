namespace EventManager.Application.Exceptions;

public interface IExceptionMapper
{
    ExceptionMapping? Map(Exception exception);
}
