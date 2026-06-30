using EventManager.Application.Interfaces;
using EventManager.Application.Model.Filters;
using EventManager.Domain.Constants;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Model.Validators;

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
