using FluentAssertions;
using RaceDay.API.Models.DTOs.Enrolments;
using RaceDay.API.Models.DTOs.Results;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;
using Xunit;

namespace RaceDay.Tests { 

public class ResultTests
{
    [Fact]
    public async Task JarredAsOrganiserCanCaptureResult()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var (eventId, categoryId) = await TestHelper.SeedEventAndCategory(context, organiserId);

        var enrolmentService = new EnrolmentService(context);
        var enrolment = await enrolmentService.EnrolAsync(participantId, categoryId, new EnrolDto());

        var resultService = new ResultService(context);
        var result = await resultService.CreateAsync(enrolment!.EnrolmentId, new CreateResultDto
        {
            FinishTime = TimeSpan.FromHours(3.5),
            OverallPosition = 15,
            CategoryPosition = 2
        });

        result.Should().NotBeNull();
        result!.OverallPosition.Should().Be(15);
        result.CategoryPosition.Should().Be(2);
    }

    [Fact]
    public async Task JarredCannotCaptureTwoResultsForSameEnrolment()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var (eventId, categoryId) = await TestHelper.SeedEventAndCategory(context, organiserId);

        var enrolmentService = new EnrolmentService(context);
        var enrolment = await enrolmentService.EnrolAsync(participantId, categoryId, new EnrolDto());

        var resultService = new ResultService(context);
        await resultService.CreateAsync(enrolment!.EnrolmentId, new CreateResultDto
        {
            FinishTime = TimeSpan.FromHours(3.5),
            OverallPosition = 15,
            CategoryPosition = 2
        });

        var secondAttempt = await resultService.CreateAsync(enrolment.EnrolmentId, new CreateResultDto
        {
            FinishTime = TimeSpan.FromHours(4.0),
            OverallPosition = 20,
            CategoryPosition = 5
        });

        secondAttempt.Should().BeNull();
    }

    [Fact]
    public async Task JarredCanViewHisResults()
    {
        var context = TestHelper.GetInMemoryContext();
        var organiserId = await TestHelper.SeedJarredAsOrganiser(context);
        var participantId = await TestHelper.SeedJarredAsParticipant(context);
        var (eventId, categoryId) = await TestHelper.SeedEventAndCategory(context, organiserId);

        var enrolmentService = new EnrolmentService(context);
        var enrolment = await enrolmentService.EnrolAsync(participantId, categoryId, new EnrolDto());

        var resultService = new ResultService(context);
        await resultService.CreateAsync(enrolment!.EnrolmentId, new CreateResultDto
        {
            FinishTime = TimeSpan.FromHours(3.5),
            OverallPosition = 15,
            CategoryPosition = 2
        });

        var myResults = await resultService.GetMyResultsAsync(participantId);

        myResults.Should().HaveCount(1);
        myResults[0].OverallPosition.Should().Be(15);
    }
}

}