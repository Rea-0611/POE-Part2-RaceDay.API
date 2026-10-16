
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Auth;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;
using Xunit;

namespace RaceDay.Tests
{

    public class AuthTests
    {
        [Fact]
        public async Task JarredCanRegisterAsOrganiser()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            var dto = new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Organiser",
                OrganisationName = "Jarred's Running Club"
            };

            var result = await authService.RegisterAsync(dto);

            result.Should().NotBeNull();
            result!.Email.Should().Be("jarred@raceday.com");
            result.Role.Should().Be("Organiser");
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task JarredCanRegisterAsParticipant()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            var dto = new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Participant",
                DateOfBirth = new DateTime(1990, 5, 15),
                ContactNumber = "0821234567"
            };

            var result = await authService.RegisterAsync(dto);

            result.Should().NotBeNull();
            result!.Role.Should().Be("Participant");
        }

        [Fact]
        public async Task JarredCannotRegisterWithDuplicateEmail()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            var dto = new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            };

            await authService.RegisterAsync(dto);
            var secondAttempt = await authService.RegisterAsync(dto);

            secondAttempt.Should().BeNull();
        }

        [Fact]
        public async Task JarredCannotRegisterWithInvalidRole()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            var dto = new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Admin"
            };

            var result = await authService.RegisterAsync(dto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task JarredCanLoginWithCorrectCredentials()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            await authService.RegisterAsync(new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            });

            var result = await authService.LoginAsync(new LoginDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!"
            });

            result.Should().NotBeNull();
            result!.Email.Should().Be("jarred@raceday.com");
        }

        [Fact]
        public async Task JarredCannotLoginWithWrongPassword()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            await authService.RegisterAsync(new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            });

            var result = await authService.LoginAsync(new LoginDto
            {
                Email = "jarred@raceday.com",
                Password = "WrongPassword!"
            });

            result.Should().BeNull();
        }

        [Fact]
        public async Task JarredCannotLoginWithUnknownEmail()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            var result = await authService.LoginAsync(new LoginDto
            {
                Email = "unknown@raceday.com",
                Password = "Password123!"
            });

            result.Should().BeNull();
        }

        [Fact]
        public async Task JarredPasswordIsHashed()
        {
            var context = TestHelper.GetInMemoryContext();
            var tokenService = new TokenService(TestHelper.GetTestConfig());
            var authService = new AuthService(context, tokenService);

            await authService.RegisterAsync(new RegisterDto
            {
                Email = "jarred@raceday.com",
                Password = "Jarred123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            });

            var storedUser = context.Users.First();

            storedUser.PasswordHash.Should().NotBe("Jarred123!");
            storedUser.PasswordHash.Should().StartWith("$2");
        }
    }

}
