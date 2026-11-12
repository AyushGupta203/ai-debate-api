using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIDebate.Api.Services;

public class OpenRouterLlmService : ILlmService
{
    private readonly HttpClient _httpClient;

    public OpenRouterLlmService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        
        var apiKey = config["Llm:OpenRouterApiKey"] ?? throw new Exception("OpenRouter API key is missing.");

        _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "AI Debate Arena");
    }

    public async Task<string> CallAsync(string modelName, string systemPrompt, string userContent)
    {
        var requestBody = new
        {
            model = modelName,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userContent }
            },
          
            max_tokens = 1200
        };

        var jsonBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        int maxRetries = 3;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"OpenRouter API error: {errorMsg}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                if (!doc.RootElement.TryGetProperty("choices", out var choices))
                {
                    throw new Exception($"Invalid response format: {jsonResponse}");
                }

                return choices[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;
            }
            catch (HttpRequestException) when (attempt < maxRetries)
            {
                // Wait 1 second before retrying
                await Task.Delay(1000); 
            }
        }

        throw new Exception("OpenRouter API failed after multiple retries due to network issues.");
    }
}