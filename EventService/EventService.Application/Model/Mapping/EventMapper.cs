using EventService.Application.Model.DTOs;
using EventService.Domain.Entities;

namespace EventService.Application.Model.Mapping;

public static class EventMapper
{
    public static Event ToEntity(this EventDto model)
    {
        return new Event( 
            model.Title, 
            model.Description, 
            model.StartAt!.Value, 
            model.EndAt!.Value,
            model.TotalSeats);
    }

    public static FullEventDto ToDto(this Event model)
    {
        return new FullEventDto
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            StartAt = model.StartAt,
            EndAt = model.EndAt,
            TotalSeats = model.TotalSeats,
            AvailableSeats = model.AvailableSeats
        };
    }
}
