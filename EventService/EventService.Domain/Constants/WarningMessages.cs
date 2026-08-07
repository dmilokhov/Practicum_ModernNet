namespace EventService.Domain.Constants;

public static class WarningMessages
{
    public static string MessageWasHandledWarningMsg(Guid msgId) =>
        $"The message {msgId} has already been handled";
}
