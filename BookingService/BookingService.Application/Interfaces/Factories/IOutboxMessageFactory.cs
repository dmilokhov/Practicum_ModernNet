using BookingService.Domain.Entities;

namespace BookingService.Application.Interfaces.Factories;

public interface IOutboxMessageFactory
{
    OutboxMessage Create(string topic, string key, string type, string payload);
}
