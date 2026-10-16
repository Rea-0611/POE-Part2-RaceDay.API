using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay.API.Data;
using RaceDay.API.Models.Entities;

namespace RaceDay.Tests.Helpers;

public static class TestHelper
{
    /// <summary>Creates a fresh in-memory database for each test.</summary>
    public static RaceDayDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    /// <summary>Returns a test IConfiguration for JWT.</summary>
    public static IConfiguration GetTestConfig()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "TestKeyForJWTSigningThatIsAtLeast32CharactersLong!" },
            { "Jwt:Issuer", "RaceDayTest" },
            { "Jwt:Audience", "RaceDayTestClient" },
            { "Jwt:ExpiryMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    /// <summary>Seeds Jarred as an Organiser. Returns his UserId.</summary>
    public static async Task<int> SeedJarredAsOrganiser(RaceDayDbContext context)
    {
        var user = new User
        {
            Email = "jarred@raceday.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Jarred123!"),
            FullName = "Jarred Smith",
            Role = "Organiser"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.Organisers.Add(new Organiser
        {
            OrganiserId = user.UserId,
            OrganisationName = "Jarred's Running Club"
        });
        await context.SaveChangesAsync();

        return user.UserId;
    }

    /// <summary>Seeds Jarred as a Participant. Returns his UserId.</summary>
    public static async Task<int> SeedJarredAsParticipant(RaceDayDbContext context)
    {
        var user = new User
        {
            Email = "jarred@raceday.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Jarred123!"),
            FullName = "Jarred Smith",
            Role = "Participant"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.Participants.Add(new Participant
        {
            ParticipantId = user.UserId,
            DateOfBirth = new DateTime(1990, 5, 15),
            ContactNumber = "0821234567"
        });
        await context.SaveChangesAsync();

        return user.UserId;
    }

    /// <summary>Seeds an event + category for enrolment/result tests.</summary>
    public static async Task<(int eventId, int categoryId)> SeedEventAndCategory(
        RaceDayDbContext context, int organiserId)
    {
        var ev = new Event
        {
            OrganiserId = organiserId,
            EventName = "Jarred's Marathon",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Durban",
            Distance = "42.2km",
            EventType = "Run",
            EventStatus = "Open"
        };
        context.Events.Add(ev);
        await context.SaveChangesAsync();

        var category = new Category
        {
            EventId = ev.EventId,
            CategoryName = "Full Marathon",
            CategoryType = "Distance",
            EntryFee = 250.00m
        };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return (ev.EventId, category.CategoryId);
    }
}