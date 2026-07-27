using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Enums;
using EventManager.Common.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManager.Common.AspNetCore.Helpers;

public static class ControllerExtensions
{
    public static Guid GetUserId(this ControllerBase controller)
    {
        var subClaim = controller.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out Guid userId))
        {
            throw new UnauthorizedException(CommonExceptionMessages.InvalidUserIdMsg);
        }

        return userId;
    }

    public static Roles GetUserRole(this ControllerBase controller)
    {
        var subClaim = controller.User.FindFirstValue("role");

        if (string.IsNullOrEmpty(subClaim) || !Enum.TryParse(subClaim, out Roles role))
        {
            throw new UnauthorizedException(CommonExceptionMessages.InvalidUserRoleMsg);
        }

        return role;
    }
}
