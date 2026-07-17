using EventManager.Application.Interfaces;
using EventManager.Application.Model.Mapping;
using EventManager.Domain.Exceptions;
using FluentValidation;

namespace EventManager.Web.Exceptions;

public class ExceptionMapper : IExceptionMapper
{
    public ExceptionMappingModel? Map(Exception exception) => exception switch
    {
        NotFoundException nf => new(404, nf.Message),
        NoAvailableSeatsException nas => new(409, nas.Message),
        DomainValidationException ve => new(400, ve.Message),
        ValidationException fve => new(400, fve.Message),
        UnauthorizedException ue => new(401, ue.Message),
        OperationNotAllowedException nae => new(403, nae.Message),
        TryBookStartedEventException tbse => new (400, tbse.Message),
        BookingLimitOverflowException bloe => new (409, bloe.Message),
        TryChangeCancelledBookingException tce => new (400, tce.Message),
        _ => null
    };
}
