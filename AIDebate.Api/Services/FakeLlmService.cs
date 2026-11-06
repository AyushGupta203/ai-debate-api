namespace AIDebate.Api.Services;

public class FakeLlmService : ILlmService
{
    public Task<string> CallAsync(string systemPrompt, string userContent)
    {
        if (systemPrompt.Contains("PRO debater", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult("PRO: AI regulation protects users from harm.");
        }

        if (systemPrompt.Contains("CON debater", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult("CON: Centralized regulation kills innovation.");
        }

        // Judge
        return Task.FromResult(@"{
            ""outcome"": ""pro"",
            ""summary"": ""Pro cited stronger evidence."",
            ""confidence"": 0.9
        }");
    }
}