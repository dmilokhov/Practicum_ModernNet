using BookingService.Application.Interfaces.Factories;
using BookingService.Domain.Entities;

namespace BookingService.Application.Model.Factories;

public class OutboxMessageFactory : IOutboxMessageFactory
{
    public OutboxMessage Create(string topic, string key, string type, string payload) =>
        new (Guid.NewGuid(), topic, key, type, payload, DateTime.UtcNow, false);
}
