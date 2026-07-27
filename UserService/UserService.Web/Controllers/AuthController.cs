using EventManager.Common.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using UserService.Application.Interfaces.Services;

namespace UserService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ILoginService loginService) : ControllerBase
{
    /// <summary>
    /// User's registration
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="204">No content - user has been registered</response>
    /// <response code="400">Returns JSON ApiErrorResult with corresponding validation message</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult>> RegisterAsync(RegistrationCommand request, CancellationToken ct = default)
    {
        await loginService.RegisterUserAsync(request, ct);
        return NoContent();
    }

    /// <summary>
    /// User's login
    /// </summary>
    /// <param name="request">User login data</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200">Returns JSON ApiResult with user registered message</response>
    /// <response code="400">Returns JSON ApiErrorResult with corresponding validation message</response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="404">Returns JSON ApiErrorResult with corresponding message if user not found</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<string>>> LoginAsync(LoginCommand request, CancellationToken ct = default)
    {
        var token = await loginService.LoginAsync(request, ct);
        return Ok(new ApiResult<string>
        {
            Data = token,
            Message = $"User '{request.Login}' has been successfully logged in"
        });
    }
}
