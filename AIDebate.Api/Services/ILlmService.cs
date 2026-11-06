namespace AIDebate.Api.Services;
public interface ILlmService
{
  Task<string> CallAsync(string SystemPrompt, string userContext);
}