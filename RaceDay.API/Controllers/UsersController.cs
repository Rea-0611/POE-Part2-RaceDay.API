using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controller
{//start of namespace

    [ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{//start of class
    private readonly RaceDayDbContext _context;

    public UsersController(RaceDayDbContext context)
    {
        _context = context;
    }

    /// <summary>Get the currently logged-in user's profile</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users
            .Include(u => u.Organiser)
            .Include(u => u.Participant)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return NotFound();

        return Ok(new
        {
            user.UserId,
            user.Email,
            user.FullName,
            user.Role,
            organisationName = user.Organiser?.OrganisationName,
            dateOfBirth = user.Participant?.DateOfBirth,
            contactNumber = user.Participant?.ContactNumber
        });
    }

    /// <summary>Update the currently logged-in user's profile</summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users
            .Include(u => u.Organiser)
            .Include(u => u.Participant)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;

        if (user.Role == "Organiser" && user.Organiser != null && dto.OrganisationName != null)
            user.Organiser.OrganisationName = dto.OrganisationName;

        if (user.Role == "Participant" && user.Participant != null)
        {
            if (dto.ContactNumber != null) user.Participant.ContactNumber = dto.ContactNumber;
            if (dto.DateOfBirth.HasValue) user.Participant.DateOfBirth = dto.DateOfBirth;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully" });
    }
}

public class UpdateProfileDto
{
    public string? FullName { get; set; }
    public string? OrganisationName { get; set; }
    public string? ContactNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
}//end of class

}//end of namespace