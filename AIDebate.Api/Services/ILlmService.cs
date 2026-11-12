namespace AIDebate.Api.Services;
public interface ILlmService
{
  Task<string> CallAsync( string modelName,string SystemPrompt, string userContext);
}