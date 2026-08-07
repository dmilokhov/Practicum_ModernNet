namespace EventManager.Common.Core.Exceptions;

public class UnauthorizedException(string message) : ApiException(404, message);
