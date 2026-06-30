using EventManager.Application.Interfaces;
using EventManager.Application.Model.Mapping;
using EventManager.Domain.Exceptions;

namespace EventManager.Web.Exceptions;

public class ExceptionMapper : IExceptionMapper
{
    public ExceptionMappingModel? Map(Exception exception) => exception switch
    {
        NotFoundException nf => new(404, nf.Message),
        NoAvailableSeatsException nas => new(409, nas.Message),
        DomainValidationException ve => new(400, ve.Message),
        _ => null
    };
}
