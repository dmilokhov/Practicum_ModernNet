namespace EventManager.Common.Core.Exceptions;

public class NotFoundException(string message) : ApiException(404, message);
