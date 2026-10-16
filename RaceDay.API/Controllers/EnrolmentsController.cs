using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models.DTOs.Enrolments;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{//start of namespace

    [ApiController]
[Route("api")]
[Authorize]
public class EnrolmentsController : ControllerBase
    {//start of class
        private readonly IEnrolmentService _enrolmentService;
    private readonly IEventService _eventService;

    public EnrolmentsController(IEnrolmentService enrolmentService, IEventService eventService)
    {
        _enrolmentService = enrolmentService;
        _eventService = eventService;
    }

    /// <summary>Get all enrolments for the logged-in participant</summary>
    [HttpGet("participants/me/enrolments")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMyEnrolments()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _enrolmentService.GetMyEnrolmentsAsync(userId));
    }

    /// <summary>Enrol in a category (Participant only)</summary>
    [HttpPost("events/{eventId}/categories/{categoryId}/enrol")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Enrol(int eventId, int categoryId, [FromBody] EnrolDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _enrolmentService.EnrolAsync(userId, categoryId, dto);
        if (result == null)
            return BadRequest(new { error = "Enrolment failed - you may already be enrolled or the category does not exist" });
        return StatusCode(201, result);
    }

    /// <summary>Update payment status of an enrolment</summary>
    [HttpPut("enrolments/{id}")]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] EnrolDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;
        var success = await _enrolmentService.UpdatePaymentAsync(id, dto.PaymentStatus, userId, role);
        return success ? Ok(new { message = "Payment status updated" }) : Forbid();
    }

    /// <summary>Get all enrolments for an event (Organiser who owns it)</summary>
    [HttpGet("events/{eventId}/enrolments")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (!await _eventService.OrganiserOwnsEvent(eventId, userId))
            return Forbid();

        return Ok(await _enrolmentService.GetByEventAsync(eventId));
    }
}//end of class

}//end of namespace