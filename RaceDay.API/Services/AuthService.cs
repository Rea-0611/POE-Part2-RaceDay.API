using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Auth;
using RaceDay.API.Models.Entities;

namespace RaceDay.API.Services
{ //start of namespace

    public interface IAuthService
    {//start of interface
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }// end of interface

    public class AuthService : IAuthService
    {//start of class
        private readonly RaceDayDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(RaceDayDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return null;

        if (dto.Role != "Organiser" && dto.Role != "Participant")
            return null;

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (dto.Role == "Organiser")
        {
            _context.Organisers.Add(new Organiser
            {
                OrganiserId = user.UserId,
                OrganisationName = dto.OrganisationName
            });
        }
        else
        {
            _context.Participants.Add(new Participant
            {
                ParticipantId = user.UserId,
                DateOfBirth = dto.DateOfBirth,
                ContactNumber = dto.ContactNumber
            });
        }
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = _tokenService.CreateToken(user)
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = _tokenService.CreateToken(user)
        };
    }

    }//end of class 

}//end of namespace
