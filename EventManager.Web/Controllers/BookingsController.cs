using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.DTOs;
using EventManager.Web.Constants;
using EventManager.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController(IBookingService bookingService) : ControllerBase
    {
        /// <summary>
        /// Get booking data by its Guid
        /// </summary>
        /// <param name="bookingId">Guid - id of booking to search</param>
        /// <param name="ct">(optional) - cancellation token</param>
        /// <response code="200"> Returns JSON ApiResult with booking data. </response>
        /// <response code="404">Returns JSON ApiErrorResult with corresponding message if booking not found</response>
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{bookingId:guid}", Name = RouteNames.GetBookingIdRoute)]
        public async Task<ActionResult<ApiResult<BookingDto>>> GetAsync(Guid bookingId, CancellationToken ct = default)
        {
            var bookingDto = await bookingService.GetBookingByIdAsync(bookingId, ct);
            return Ok(new ApiResult<BookingDto>
            {
                Data = bookingDto,
                Message = $"Getting data of the booking: {bookingId}"
            });
        }
    }
}
