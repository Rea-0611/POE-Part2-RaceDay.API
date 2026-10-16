using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models.DTOs.Auth;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{//start of namespace

    [ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
    {//start of class
        private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Register a new user (Organiser or Participant)</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null)
            return BadRequest(new { error = "Registration failed. Email may already exist or role is invalid." });

        return StatusCode(201, result);
    }

    /// <summary>Login with email and password to receive a JWT token</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
            return Unauthorized(new { error = "Invalid email or password" });

        return Ok(result);
    }
}//end of class

}//end of namespace