using FluentAssertions;
using RaceDay.API.Models.DTOs.Events;
using RaceDay.API.Models.Entities;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;
using Xunit;

namespace RaceDay.Tests { 

public class RoleAccessTests
{
    [Fact]
    public async Task JarredAsOrganiserCannotUpdateAnotherOrganisersEvent()
    {
        var context = TestHelper.GetInMemoryContext();

        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);

        var otherUser = new User
        {
            Email = "other@raceday.com",
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

        var otherEvent = await service.CreateAsync(new CreateEventDto
        {
            EventName = "Other's Event",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Joburg",
            EventType = "Run"
        }, otherUser.UserId);

        var result = await service.UpdateAsync(otherEvent.EventId, new UpdateEventDto
        {
            EventName = "Hacked!",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Joburg",
            EventType = "Run"
        }, jarredId);

        result.Should().BeNull("Jarred should not modify events he did not create");
    }

    [Fact]
    public async Task JarredAsOrganiserCannotDeleteAnotherOrganisersEvent()
    {
        var context = TestHelper.GetInMemoryContext();
        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);

        var otherUser = new User
        {
            Email = "other@raceday.com",
            PasswordHash = "hash",
            FullName = "Other",
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
        var otherEvent = await service.CreateAsync(new CreateEventDto
        {
            EventName = "Other's Event",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Joburg",
            EventType = "Run"
        }, otherUser.UserId);

        var success = await service.DeleteAsync(otherEvent.EventId, jarredId);

        success.Should().BeFalse();
    }

    [Fact]
    public async Task JarredAsParticipantCannotUpdateEventsHeDidNotCreate()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);

        var service = new EventService(context);
        var ev = await service.CreateAsync(new CreateEventDto
        {
            EventName = "Jarred's Event",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Durban",
            EventType = "Run"
        }, organiserId);

        var result = await service.UpdateAsync(ev.EventId, new UpdateEventDto
        {
            EventName = "Hacked",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Durban",
            EventType = "Run"
        }, participantId);

        result.Should().BeNull();
    }
}

}