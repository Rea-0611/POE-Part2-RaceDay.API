using FluentAssertions;
using RaceDay.API.Models.DTOs.Enrolments;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;
using Xunit;

namespace RaceDay.Tests { 

public class EnrolmentTests
{
    [Fact]
    public async Task JarredAsParticipantCanEnrolInCategory()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var (eventId, categoryId) = await TestHelper.SeedEventAndCategory(context, organiserId);

        var service = new EnrolmentService(context);
        var result = await service.EnrolAsync(participantId, categoryId, new EnrolDto
        {
            PaymentStatus = "Pending"
        });

        result.Should().NotBeNull();
        result!.CategoryName.Should().Be("Full Marathon");
        result.PaymentStatus.Should().Be("Pending");
    }

    [Fact]
    public async Task JarredCannotEnrolInSameCategoryTwice()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var (eventId, categoryId) = await TestHelper.SeedEventAndCategory(context, organiserId);

        var service = new EnrolmentService(context);
        await service.EnrolAsync(participantId, categoryId, new EnrolDto());

        var secondAttempt = await service.EnrolAsync(participantId, categoryId, new EnrolDto());

        secondAttempt.Should().BeNull("Jarred should not enrol twice in the same category");
    }

    [Fact]
    public async Task JarredCanViewHisOwnEnrolments()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var (eventId, categoryId) = await TestHelper.SeedEventAndCategory(context, organiserId);

        var service = new EnrolmentService(context);
        await service.EnrolAsync(participantId, categoryId, new EnrolDto());

        var myEnrolments = await service.GetMyEnrolmentsAsync(participantId);

        myEnrolments.Should().HaveCount(1);
        myEnrolments[0].EventName.Should().Be("Jarred's Marathon");
    }

    [Fact]
    public async Task JarredCannotEnrolInNonExistentCategory()
    {
        var context = TestHelper.GetInMemoryContext();
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var service = new EnrolmentService(context);

        var result = await service.EnrolAsync(participantId, 9999, new EnrolDto());

        result.Should().BeNull();
    }
}

}