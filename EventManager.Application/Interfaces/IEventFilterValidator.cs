using EventManager.Application.Model.Filters;

namespace EventManager.Application.Interfaces;

public interface IEventFilterValidator
{
    public void Validate(EventFilter filter);
}
