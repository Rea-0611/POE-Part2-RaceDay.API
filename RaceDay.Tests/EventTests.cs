using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Events;
using RaceDay.API.Models.Entities;
using RaceDay.API.Services;
using Xunit;

namespace RaceDay.Tests;

public class EventTests
{
    private RaceDayDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    private async Task<int> SeedOrganiserAsync(RaceDayDbContext context)
    {
        var user = new User
        {
            Email = "jarred@test.com",
            PasswordHash = "hash",
            FullName = "Jarred Smith",
            Role = "Organiser"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.Organisers.Add(new Organiser
        {
            OrganiserId = user.UserId,
            OrganisationName = "Jarred's Club"
        });
        await context.SaveChangesAsync();

        return user.UserId;
    }

    [Fact]
    public async Task JarredAsOrganiserCanCreateEvent()
    {
        var context = GetInMemoryContext();
        var organiserId = await SeedOrganiserAsync(context);
        var service = new EventService(context);

        var dto = new CreateEventDto
        {
            EventName = "Jarred's Marathon",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Durban",
            Distance = "42.2km",
            EventType = "Run",
            EventStatus = "Open"
        };

        var result = await service.CreateAsync(dto, organiserId);

        result.Should().NotBeNull();
        result.EventName.Should().Be("Jarred's Marathon");
        result.EventType.Should().Be("Run");
    }

    [Fact]
    public async Task JarredCanViewAllEvents()
    {
        var context = GetInMemoryContext();
        var organiserId = await SeedOrganiserAsync(context);
        var service = new EventService(context);

        await service.CreateAsync(new CreateEventDto
        {
            EventName = "Event 1",
            EventDate = DateTime.UtcNow.AddDays(10),
            Location = "Cape Town",
            EventType = "Cycle"
        }, organiserId);

        var events = await service.GetAllAsync();

        events.Should().HaveCount(1);
        events[0].EventName.Should().Be("Event 1");
    }

    [Fact]
    public async Task JarredCannotUpdateEventHeDidNotCreate()
    {
        var context = GetInMemoryContext();
        var jarredId = await SeedOrganiserAsync(context);

        // Seed a second organiser
        var otherUser = new User
        {
            Email = "other@test.com",
            PasswordHash = "hash",
            FullName = "Other Organiser",
            Role = "Organiser"
        };
        context.Users.Add(otherUser);
        await context.SaveChangesAsync();
        context.Organisers.Add(new Organiser
        {
            OrganiserId = otherUser.UserId,
            OrganisationName = "Other Club"
        });
        await context.SaveChangesAsync();

        var service = new EventService(context);
        var ev = await service.CreateAsync(new CreateEventDto
        {
            EventName = "Other's Event",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Joburg"
        }, otherUser.UserId);

        // Jarred tries to update Other's event
        var result = await service.UpdateAsync(ev.EventId, new UpdateEventDto
        {
            EventName = "Hacked Name",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Joburg"
        }, jarredId);

        result.Should().BeNull();
    }
}