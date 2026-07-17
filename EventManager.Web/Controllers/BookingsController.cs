using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Responses;
using EventManager.Web.Constants;
using EventManager.Web.Contracts;
using EventManager.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Web.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Get booking data by its Guid
    /// </summary>
    /// <param name="bookingId">Guid - id of booking to search</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200"> Returns JSON ApiResult with booking data. </response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="404">Returns JSON ApiErrorResult with corresponding message if booking not found</response>
    [ProducesResponseType(typeof(ApiResult<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [HttpGet("{bookingId:guid}", Name = RouteNames.GetBookingIdRoute)]
    public async Task<ActionResult<ApiResult<BookingResponse>>> GetAsync(Guid bookingId, CancellationToken ct = default)
    {
        var request = new GetBookingByIdCommand(bookingId, this.GetUserId(), this.GetUserRole());
        var bookingDto = await bookingService.GetBookingByIdAsync(request, ct);
        return Ok(new ApiResult<BookingResponse>
        {
            Data = bookingDto,
            Message = $"Getting data of the booking: {bookingId}"
        });
    }

    /// <summary>
    /// Cancel Booking
    /// </summary>
    /// <param name="bookingId">ID of a booking to cancel</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200">Returns JSON ApiResult with successful delete message</response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="403">Returns JSON ApiErrorResult with corresponding message if access is forbidden</response>
    /// <response code="404">Returns JSON ApiErrorResult with corresponding message if event not found</response>
    [HttpPost("{bookingId:guid}/Cancel")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResult>> Cancel([FromRoute] Guid bookingId, CancellationToken ct = default)
    {
        var request = new CancelBookingCommand(bookingId, this.GetUserId(), this.GetUserRole());

        await bookingService.CancelBookingAsync(request, ct);
        return Ok(new ApiResult
        {
            Message = $"Booking {request.BookingId} has been cancelled"
        });
    }
}
