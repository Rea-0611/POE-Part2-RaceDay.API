using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models.DTOs.Categories;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{//start of namespace

    [ApiController]
[Route("api")]
public class CategoriesController : ControllerBase
    {//start of class
        private readonly ICategoryService _categoryService;
    private readonly IEventService _eventService;

    public CategoriesController(ICategoryService categoryService, IEventService eventService)
    {
        _categoryService = categoryService;
        _eventService = eventService;
    }

    /// <summary>Get all categories for an event (public)</summary>
    [HttpGet("events/{eventId}/categories")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var eventItem = await _eventService.GetByIdAsync(eventId);
        if (eventItem == null) return NotFound(new { error = "Event not found" });
        return Ok(await _categoryService.GetByEventAsync(eventId));
    }

    /// <summary>Add a category to an event (Organiser who owns the event)</summary>
    [HttpPost("events/{eventId}/categories")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, [FromBody] CreateCategoryDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (!await _eventService.OrganiserOwnsEvent(eventId, userId))
            return Forbid();

        var result = await _categoryService.CreateAsync(eventId, dto);
        return StatusCode(201, result);
    }

    /// <summary>Update a category (Organiser who owns the parent event)</summary>
    [HttpPut("categories/{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Delete a category (fails if enrolments exist)</summary>
    [HttpDelete("categories/{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _categoryService.DeleteAsync(id);
        return success
            ? NoContent()
            : Conflict(new { error = "Cannot delete category - it may have enrolments or does not exist" });
    }
}//end of class

}//end of namespace