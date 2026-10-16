using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models.DTOs.Events;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{//start of namespace

    [ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
    {//start of class
        private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>Get all events (public)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _eventService.GetAllAsync());

    /// <summary>Get a single event by ID (public)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _eventService.GetByIdAsync(id);
        return result == null ? NotFound(new { error = "Event not found" }) : Ok(result);
    }

    /// <summary>Create a new event (Organiser only)</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _eventService.CreateAsync(dto, userId);
        return StatusCode(201, result);
    }

    /// <summary>Update an event (Organiser who created it only)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _eventService.UpdateAsync(id, dto, userId);
        if (result == null)
            return Forbid();
        return Ok(result);
    }

    /// <summary>Delete an event (Organiser who created it only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _eventService.DeleteAsync(id, userId);
        return success ? NoContent() : Forbid();
    }
}//end of class
}//end of namespace
