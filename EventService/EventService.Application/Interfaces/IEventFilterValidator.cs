using EventService.Application.Model.Filters;

namespace EventService.Application.Interfaces;

public interface IEventFilterValidator
{
    public void Validate(EventFilter filter);
}
