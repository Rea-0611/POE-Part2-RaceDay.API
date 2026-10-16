using FluentAssertions;
using RaceDay.API.Models.DTOs.Events;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;
using Xunit;

namespace RaceDay.Tests {

public class EventTests
{
    [Fact]
    public async Task JarredAsOrganiserCanCreateEvent()
    {
        var context = TestHelper.GetInMemoryContext();
        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);
        var service = new EventService(context);

        var dto = new CreateEventDto
        {
            EventName = "Jarred's Marathon",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Durban",
            Description = "Jarred's first event",
            Distance = "42.2km",
            EventType = "Run",
            EventStatus = "Open"
        };

        var result = await service.CreateAsync(dto, jarredId);

        result.Should().NotBeNull();
        result.EventName.Should().Be("Jarred's Marathon");
        result.Distance.Should().Be("42.2km");
        result.EventType.Should().Be("Run");
    }

    [Fact]
    public async Task JarredCanViewAllEvents()
    {
        var context = TestHelper.GetInMemoryContext();
        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);
        var service = new EventService(context);

        await service.CreateAsync(new CreateEventDto
        {
            EventName = "Jarred's Event 1",
            EventDate = DateTime.UtcNow.AddDays(10),
            Location = "Cape Town",
            EventType = "Cycle"
        }, jarredId);

        var events = await service.GetAllAsync();

        events.Should().HaveCount(1);
        events[0].EventName.Should().Be("Jarred's Event 1");
    }

    [Fact]
    public async Task JarredCanViewEventById()
    {
        var context = TestHelper.GetInMemoryContext();
        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);
        var service = new EventService(context);

        var created = await service.CreateAsync(new CreateEventDto
        {
            EventName = "Jarred's Race",
            EventDate = DateTime.UtcNow.AddDays(5),
            Location = "Joburg",
            EventType = "Run"
        }, jarredId);

        var result = await service.GetByIdAsync(created.EventId);

        result.Should().NotBeNull();
        result!.EventName.Should().Be("Jarred's Race");
    }

    [Fact]
    public async Task JarredCanUpdateHisOwnEvent()
    {
        var context = TestHelper.GetInMemoryContext();
        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);
        var service = new EventService(context);

        var created = await service.CreateAsync(new CreateEventDto
        {
            EventName = "Old Name",
            EventDate = DateTime.UtcNow.AddDays(10),
            Location = "Durban",
            EventType = "Run"
        }, jarredId);

        var updated = await service.UpdateAsync(created.EventId, new UpdateEventDto
        {
            EventName = "New Name",
            EventDate = DateTime.UtcNow.AddDays(15),
            Location = "Durban",
            EventType = "Run"
        }, jarredId);

        updated.Should().NotBeNull();
        updated!.EventName.Should().Be("New Name");
    }

    [Fact]
    public async Task JarredCanDeleteHisOwnEvent()
    {
        var context = TestHelper.GetInMemoryContext();
        var jarredId = await TestHelper.SeedJarredAsOrganiser(context);
        var service = new EventService(context);

        var created = await service.CreateAsync(new CreateEventDto
        {
            EventName = "To Be Deleted",
            EventDate = DateTime.UtcNow.AddDays(10),
            Location = "Durban",
            EventType = "Run"
        }, jarredId);

        var success = await service.DeleteAsync(created.EventId, jarredId);

        success.Should().BeTrue();
        var all = await service.GetAllAsync();
        all.Should().BeEmpty();
    }
}

}