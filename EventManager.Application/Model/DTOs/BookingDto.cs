using EventManager.Domain.Constants;
using EventManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Model.DTOs;

public class BookingDto 
{
    [Required(ErrorMessage = ValidationMessages.IdIsRequiredMsg)]
    public Guid Id { get; init; }

    [Required(ErrorMessage = ValidationMessages.EventIdIsRequiredMsg)]
    public required Guid EventId { get; set; }

    [Required(ErrorMessage = ValidationMessages.BookingStatusIsRequiredMsg)]
    public BookingStatuses Status { get; set; }

    [Required(ErrorMessage = ValidationMessages.CreatedAtIsRequiredMsg)]
    public DateTime? CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }
}
