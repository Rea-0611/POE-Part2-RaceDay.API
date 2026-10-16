using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models.DTOs.Results;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{//start of namespace

    [ApiController]
[Route("api")]
public class ResultsController : ControllerBase
    {// start of class
        private readonly IResultService _resultService;

    public ResultsController(IResultService resultService)
    {
        _resultService = resultService;
    }

    /// <summary>Get all results for an event (public)</summary>
    [HttpGet("events/{eventId}/results")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        return Ok(await _resultService.GetByEventAsync(eventId));
    }

    /// <summary>Get my results (Participant only)</summary>
    [HttpGet("participants/me/results")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMyResults()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _resultService.GetMyResultsAsync(userId));
    }

    /// <summary>Capture a result (Organiser only)</summary>
    [HttpPost("enrolments/{enrolmentId}/results")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int enrolmentId, [FromBody] CreateResultDto dto)
    {
        var result = await _resultService.CreateAsync(enrolmentId, dto);
        if (result == null)
            return Conflict(new { error = "Result already exists or enrolment not found" });
        return StatusCode(201, result);
    }

    /// <summary>Update a result (Organiser only)</summary>
    [HttpPut("results/{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateResultDto dto)
    {
        var result = await _resultService.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Delete a result (Organiser only)</summary>
    [HttpDelete("results/{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _resultService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}//end of class

}//end of namespace