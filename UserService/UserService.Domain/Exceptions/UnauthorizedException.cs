using EventManager.Common.Core.Exceptions;

namespace UserService.Domain.Exceptions;

public class UnauthorizedException(string message) : ApiException(404, message);
