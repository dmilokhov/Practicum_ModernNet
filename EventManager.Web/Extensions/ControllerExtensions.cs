using System.Security.Claims;
using EventManager.Domain.Constants;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Web.Extensions;

public static class ControllerExtensions
{
    public static Guid GetUserId(this ControllerBase controller)
    {
        var subClaim = controller.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out Guid userId))
        {
            throw new UnauthorizedException(ExceptionMessages.InvalidUserIdMsg);
        }

        return userId;
    }

    public static Roles GetUserRole(this ControllerBase controller)
    {
        var subClaim = controller.User.FindFirstValue("role");

        if (string.IsNullOrEmpty(subClaim) || !Enum.TryParse(subClaim, out Roles role))
        {
            throw new UnauthorizedException(ExceptionMessages.InvalidUserRoleMsg);
        }

        return role;
    }
}
