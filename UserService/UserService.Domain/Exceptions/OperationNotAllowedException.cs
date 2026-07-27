using EventManager.Common.Core.Exceptions;

namespace UserService.Domain.Exceptions;

public class OperationNotAllowedException(string message) : ApiException(403, message);
