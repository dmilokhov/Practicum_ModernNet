using EventManager.Common.Core.Contracts;

namespace BookingService.Application.Interfaces.Messaging;

public interface IBookingEventsPublisher 
{
    Task PublishAsync<TMessage>(string key, TMessage msg, CancellationToken ct = default) where TMessage : class;
}
