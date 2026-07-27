namespace EventManager.Domain.Constants;

public static class ValidationMessages
{
    public const string BookingStatusIsRequiredMsg = "BookingStatus is required";
    public const string CreatedAtIsRequiredMsg = "CreatedAt is required";
    public const string EndAtIsRequiredMsg = "EndAt is required";
    public const string EndDateLaterThanStartMsg = "End date/time must be later than start one";
    public const string EventIdIsRequiredMsg = "EventId is required";
    public const string IdIsRequiredMsg = "Id is required";
    public const string LoginIsRequiredMsg = "Login is required";
    public const string PageMustBeAboveOrEqualOne = "Page must be greater than or equal to 1";
    public const string PageSizeMustBeAboveOrEqualOne = "PageSize must be greater than or equal to 1";
    public const string PasswordIsRequiredMsg = "Password is required";
    public const string PasswordTooWeakMsg = 
        "Password must be at least 7 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.";
    public const string StartAtIsRequiredMsg = "StartAt is required";
    public const string TitleFilterWithoutSpacesMsg = "Title filter should not contain only white spaces";
    public const string TitleIsRequiredMsg = "Title is required";
    public const string TotalSeatsAboveZeroMsg = "TotalSeats should be above zero";
    public const string TotalSeatsIsRequiredMsg = "TotalSeats is required";
    public const string UserAlreadyExistsMsg = "User with the same login already exist";
}
