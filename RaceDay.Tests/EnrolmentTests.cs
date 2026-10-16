using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Enrolments;
using RaceDay.API.Models.Entities;
using RaceDay.API.Services;
using Xunit;

namespace RaceDay.Tests;

public class EnrolmentTests
{
    private RaceDayDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    private async Task<(int participantId, int categoryId)> SeedDataAsync(RaceDayDbContext context)
    {
        // Organiser
        var orgUser = new User { Email = "org@test.com", PasswordHash = "h", FullName = "Org", Role = "Organiser" };
        context.Users.Add(orgUser);
        await context.SaveChangesAsync();
        context.Organisers.Add(new Organiser { OrganiserId = orgUser.UserId, OrganisationName = "Club" });
        await context.SaveChangesAsync();

        // Event
        var ev = new Event
        {
            OrganiserId = orgUser.UserId,
            EventName = "Test Event",
            EventDate = DateTime.UtcNow.AddDays(10),
            Location = "Durban",
            EventType = "Run",
            EventStatus = "Open"
        };
        context.Events.Add(ev);
        await context.SaveChangesAsync();

        // Category
        var cat = new Category
        {
            EventId = ev.EventId,
            CategoryName = "10km",
            CategoryType = "Distance",
            EntryFee = 100
        };
        context.Categories.Add(cat);
        await context.SaveChangesAsync();

        // Participant (Jarred)
        var partUser = new User { Email = "jarred@test.com", PasswordHash = "h", FullName = "Jarred", Role = "Participant" };
        context.Users.Add(partUser);
        await context.SaveChangesAsync();
        context.Participants.Add(new Participant { ParticipantId = partUser.UserId });
        await context.SaveChangesAsync();

        return (partUser.UserId, cat.CategoryId);
    }

    [Fact]
    public async Task JarredAsParticipantCanEnrolInCategory()
    {
        var context = GetInMemoryContext();
        var (participantId, categoryId) = await SeedDataAsync(context);
        var service = new EnrolmentService(context);

        var result = await service.EnrolAsync(participantId, categoryId, new EnrolDto { PaymentStatus = "Pending" });

        result.Should().NotBeNull();
        result!.ParticipantName.Should().Be("Jarred");
        result.PaymentStatus.Should().Be("Pending");
    }

    [Fact]
    public async Task JarredCannotEnrolInSameCategoryTwice()
    {
        var context = GetInMemoryContext();
        var (participantId, categoryId) = await SeedDataAsync(context);
        var service = new EnrolmentService(context);

        await service.EnrolAsync(participantId, categoryId, new EnrolDto());

        // Second attempt should fail
        var result = await service.EnrolAsync(participantId, categoryId, new EnrolDto());

        result.Should().BeNull();
    }
}