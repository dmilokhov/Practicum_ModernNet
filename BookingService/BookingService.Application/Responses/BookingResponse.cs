using BookingService.Domain.Enums;

namespace BookingService.Application.Responses;

public class BookingResponse 
{
    public Guid Id { get; init; }
    public required Guid EventId { get; set; }
    public required Guid UserId { get; set; }
    public BookingStatuses Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int SeatsAmount { get; set; }
}
