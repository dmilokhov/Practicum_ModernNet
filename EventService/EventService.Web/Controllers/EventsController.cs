using EventManager.Common.AspNetCore;
using EventManager.Common.AspNetCore.Helpers;
using EventService.Application.Interfaces.Services;
using EventService.Application.Model.DTOs;
using EventService.Application.Model.Filters;
using EventService.Application.Responses;
using EventService.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController(IEventCrudService eventService) : ControllerBase
{
    /// <summary>
    /// Get events
    /// </summary>
    /// <param name="filter">
    /// Optional query filters:
    /// - Title: filters by title (case-insensitive, contains)
    /// - From: start date (inclusive)
    /// - To: end date (inclusive)
    /// </param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200"> Returns JSON ApiResult with events data. 
    /// If there are no any - empty list with corresponding message</response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    [ProducesResponseType(typeof(ApiResult<PagedResponse<FullEventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResponse<FullEventDto>>>> GetAll([FromQuery]EventFilter filter, 
        CancellationToken ct = default)
    {
        var data = await eventService.GetEventsAsync(filter, ct);
        var msg = data.TotalItems > 0 ? "Getting events" : "There are no events";

        return Ok(new ApiResult<PagedResponse<FullEventDto>>
        {
            Data = data,
            Message = msg
        });
    }

    /// <summary>
    /// Get an event by its Guid
    /// </summary>
    /// <param name="id">Guid - id of an event to search</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200"> Returns JSON ApiResult with an event data. </response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="404">Returns JSON ApiErrorResult with corresponding message if event not found</response>
    [ProducesResponseType(typeof(ApiResult<FullEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [HttpGet("{id:guid}", Name = RouteNames.GetAsyncRoute)]
    public async Task<ActionResult<ApiResult<FullEventDto>>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var eventDto = await eventService.GetEventAsync(id, ct);
        return Ok(new ApiResult<FullEventDto>
        {
            Data = eventDto,
            Message = $"Getting data of the event: {id}"
        });
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    /// <param name="eventDto">Data required to create an event</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="201">Returns JSON ApiResult with created event data</response>
    /// <response code="400">Returns JSON ApiErrorResult with corresponding message if there are validation errors</response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="403">Returns JSON ApiErrorResult with corresponding message if access is forbidden</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResult<FullEventDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status403Forbidden)]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResult<FullEventDto>>> CreateAsync([FromBody] EventDto eventDto, 
        CancellationToken ct = default)
    {
        var createdEvent = await eventService.AddEventAsync(eventDto, ct);
        return CreatedAtRoute(RouteNames.GetAsyncRoute, new { id = createdEvent.Id }, new ApiResult<FullEventDto>
        {
            Data = createdEvent,
            Message = $"Event created: {createdEvent.Id}"
        });
    }


    /// <summary>
    /// Update an existing event by its Guid
    /// </summary>
    /// <param name="id">Guid - id of an event to update</param>
    /// <param name="eventDto">Updated event data</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200">Returns JSON ApiResult with successful update message</response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="400">Returns JSON ApiErrorResult with corresponding message if there are validation errors</response>
    /// <response code="403">Returns JSON ApiErrorResult with corresponding message if access is forbidden</response>
    /// <response code="404">Returns JSON ApiErrorResult with corresponding message if event not found</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResult>> UpdateAsync(Guid id, [FromBody] EventDto eventDto, CancellationToken ct = default)
    {
        await eventService.UpdateEventAsync(id, eventDto, ct);
        return Ok(new ApiResult { Message = $"Event updated: {id}" });
    }

    /// <summary>
    /// Delete an event by its Guid
    /// </summary>
    /// <param name="id">Guid - id of an event to delete</param>
    /// <param name="ct">(optional) - cancellation token</param>
    /// <response code="200">Returns JSON ApiResult with successful delete message</response>
    /// <response code="401">Returns JSON ApiErrorResult with corresponding message if user is unauthorized</response>
    /// <response code="403">Returns JSON ApiErrorResult with corresponding message if access is forbidden</response>
    /// <response code="404">Returns JSON ApiErrorResult with corresponding message if event not found</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResult), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResult>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await eventService.DeleteEventAsync(id, ct);
        return Ok(new ApiResult { Message = $"Event deleted: {id}" });
    }
}
