using EventService.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace EventService.Application.Model.DTOs;

public class EventDto : IValidatableObject
{
    [Required(AllowEmptyStrings = false, ErrorMessage = ValidationMessages.TitleIsRequiredMsg)]
    public required string Title { get; set; }
    public string? Description { get; set; }

    [Required(ErrorMessage = ValidationMessages.StartAtIsRequiredMsg)]
    public DateTime? StartAt { get; set; }

    [Required(ErrorMessage = ValidationMessages.EndAtIsRequiredMsg)]
    public DateTime? EndAt { get; set; }

    [Required(ErrorMessage = ValidationMessages.TotalSeatsIsRequiredMsg)]
    public int TotalSeats { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt <= StartAt)
        {
            yield return new ValidationResult(ValidationMessages.EndDateLaterThanStartMsg, [nameof(EndAt)]);
        }

        if (TotalSeats <= 0)
        {
            yield return new ValidationResult(ValidationMessages.TotalSeatsAboveZeroMsg, [nameof(TotalSeats)]);
        }
    }
}

public class FullEventDto : EventDto
{
    public Guid Id { get; init; }
    public int AvailableSeats { get; set; }
}
