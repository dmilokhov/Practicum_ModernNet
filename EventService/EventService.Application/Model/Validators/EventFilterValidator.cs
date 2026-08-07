using EventService.Application.Interfaces;
using EventService.Application.Model.Filters;
using EventService.Domain.Constants;
using EventManager.Common.Core.Exceptions;

namespace EventService.Application.Model.Validators;

public class EventFilterValidator : IEventFilterValidator
{
    public void Validate(EventFilter filter)
    {
        if (filter.Page < 1)
        {
            throw new DomainValidationException(ValidationMessages.PageMustBeAboveOrEqualOne);
        }

        if (filter.PageSize < 1)
        {
            throw new DomainValidationException(ValidationMessages.PageSizeMustBeAboveOrEqualOne);
        }

        if (filter.Title != null && filter.Title.All(char.IsWhiteSpace))
        {
            throw new DomainValidationException(ValidationMessages.TitleFilterWithoutSpacesMsg);
        }

        if (filter is { From: not null, To: not null } && filter.To <= filter.From)
        {
            throw new DomainValidationException(ValidationMessages.EndDateLaterThanStartMsg);
        }
    }
}
