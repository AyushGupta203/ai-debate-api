namespace AIDebate.Api.Services;

public class ArenaRouterService : ILlmService
{
    private readonly GroqLlmService _groq;
    private readonly OpenRouterLlmService _openRouter;

    // Sirf Groq aur OpenRouter ko inject kar rahe hain
    public ArenaRouterService(GroqLlmService groq, OpenRouterLlmService openRouter)
    {
        _groq = groq;
        _openRouter = openRouter;
    }

    public async Task<string> CallAsync(string modelName, string systemPrompt, string userContent)
    {
        // 1. Check for Groq
        if (modelName.StartsWith("groq/"))
        {
            var actualModel = modelName.Replace("groq/", "");
            return await _groq.CallAsync(actualModel, systemPrompt, userContent);
        }
        
        // 2. Check for OpenRouter
        if (modelName.StartsWith("openrouter/"))
        {
            var actualModel = modelName.Replace("openrouter/", "");
            return await _openRouter.CallAsync(actualModel, systemPrompt, userContent);
        }


        throw new Exception($"Invalid model name format: {modelName}. Use 'groq/' or 'openrouter/'.");
    }
}