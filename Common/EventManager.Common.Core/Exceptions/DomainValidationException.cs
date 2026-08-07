namespace EventManager.Common.Core.Exceptions;

public class DomainValidationException(string message) : ApiException(400, message);
