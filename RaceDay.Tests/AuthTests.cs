using FluentAssertions;
using RaceDay.API.Models.DTOs.Auth;        
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;
using RaceDay.API.Data;
using Microsoft.EntityFrameworkCore;              
using Microsoft.Extensions.Configuration;         
using RaceDay.API.Data;                           
using Xunit;

namespace RaceDay.Tests
{

    public class AuthTests
    {
        private RaceDayDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        [Fact]
        public async Task JarredCanRegisterAsOrganiser()
        {
            // Arrange
            var context = GetInMemoryContext();
            var tokenService = new TokenService(GetConfig());
            var authService = new AuthService(context, tokenService);

            var dto = new RegisterDto
            {
                Email = "jarred@test.com",
                Password = "Password123!",
                FullName = "Jarred Smith",
                Role = "Organiser",
                OrganisationName = "Jarred's Club"
            };

            // Act
            var result = await authService.RegisterAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("jarred@test.com");
            result.Role.Should().Be("Organiser");
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task JarredCannotRegisterWithDuplicateEmail()
        {
            // Arrange
            var context = GetInMemoryContext();
            var tokenService = new TokenService(GetConfig());
            var authService = new AuthService(context, tokenService);

            var dto = new RegisterDto
            {
                Email = "jarred@test.com",
                Password = "Password123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            };

            await authService.RegisterAsync(dto);

            // Act
            var secondAttempt = await authService.RegisterAsync(dto);

            // Assert
            secondAttempt.Should().BeNull();
        }

        [Fact]
        public async Task JarredCanLoginWithCorrectCredentials()
        {
            // Arrange
            var context = GetInMemoryContext();
            var tokenService = new TokenService(GetConfig());
            var authService = new AuthService(context, tokenService);

            await authService.RegisterAsync(new RegisterDto
            {
                Email = "jarred@test.com",
                Password = "Password123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            });

            // Act
            var result = await authService.LoginAsync(new LoginDto
            {
                Email = "jarred@test.com",
                Password = "Password123!"
            });

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("jarred@test.com");
        }

        [Fact]
        public async Task JarredCannotLoginWithWrongPassword()
        {
            // Arrange
            var context = GetInMemoryContext();
            var tokenService = new TokenService(GetConfig());
            var authService = new AuthService(context, tokenService);

            await authService.RegisterAsync(new RegisterDto
            {
                Email = "jarred@test.com",
                Password = "Password123!",
                FullName = "Jarred Smith",
                Role = "Organiser"
            });

            // Act
            var result = await authService.LoginAsync(new LoginDto
            {
                Email = "jarred@test.com",
                Password = "WrongPassword!"
            });

            // Assert
            result.Should().BeNull();
        }

        private static IConfiguration GetConfig()
        {
            var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "TestKeyForJWTSigningThatIsAtLeast32CharactersLong!" },
            { "Jwt:Issuer", "RaceDayTest" },
            { "Jwt:Audience", "RaceDayTestClient" },
            { "Jwt:ExpiryMinutes", "60" }
        };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
    }

}
