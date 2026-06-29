using EventManager.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Exceptions;

public class ExceptionMapper : IExceptionMapper
{
    public ExceptionMapping? Map(Exception exception) => exception switch
    {
        NotFoundException nf => new(404, nf.Message),
        NoAvailableSeatsException nas => new(409, nas.Message),
        ValidationException ve => new(400, ve.Message),
        _ => null
    };
}
