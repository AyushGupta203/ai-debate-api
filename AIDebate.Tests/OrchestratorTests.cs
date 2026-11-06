using Xunit;
using Moq;
using AIDebate.Api.Services;
using Microsoft.EntityFrameworkCore;
using AIDebate.Api.Data;
using FluentAssertions;

namespace AIDebate.Tests;

public class OrchestratorTests
{
    [Fact]
    public async Task RunAsync_ShouldReturnValidVerdict()
    {
        // ARRANGE
        var mockLlm = new Mock<ILlmService>();

        mockLlm.Setup(x => x.CallAsync(It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync((string systemPrompt, string userQuestion) =>
               {
                   if (systemPrompt.Contains("judge"))
                       return "{ \"outcome\": \"pro\", \"summary\": \"Test summary\", \"confidence\": 0.9 }";

                   return "This is a fake argument";
               });

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var fakeDb = new AppDbContext(options);

        var orchestrator = new DebateOrchestrator(mockLlm.Object, fakeDb);

        // ACT
        var result = await orchestrator.RunAsync("Should AI be regulated");

        // ASSERT
        result.Should().NotBeNull();
        result.Question.Should().Be("Should AI be regulated"); // Yahan capital R hata diya hai
        result.Status.Should().Be("completed");

        result.Turns.Should().HaveCount(2);

        result.Verdict.Should().NotBeNull();
        result.Verdict.Outcome.Should().Be("pro");
        result.Verdict.Summary.Should().Be("Test summary");
        result.Verdict.Confidence.Should().Be(0.9);

        // VERIFY
        mockLlm.Verify(x => x.CallAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
    }
}