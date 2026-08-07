namespace UserService.Application.Commands;

public class LoginCommand
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}
