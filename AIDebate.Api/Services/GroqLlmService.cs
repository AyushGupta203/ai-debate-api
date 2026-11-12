using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace AIDebate.Api.Services;

public class GroqLlmService: ILlmService
{
  private readonly HttpClient _httpClient;

  public GroqLlmService(HttpClient httpClient , IConfiguration config)
  {
    _httpClient = httpClient;

    var apiKey = config["Llm:GroqApiKey"]?? throw new Exception("Groq Api Key is Missing. Check user-Secrets");

   _httpClient.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
    
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
  }
    public async Task<string> CallAsync(string modelName, string systemPrompt, string userContent)
  {
    var requestBody = new
    {
      model  = modelName,
      messages = new[]
      {
        new {role = "system", content = systemPrompt},
        new {role = "user", content = userContent}


      },
      temperature = 0.7,
      max_completion_tokens = 2500





    };

    var jsonBody = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync("chat/completions", content);

    if (!response.IsSuccessStatusCode)
    {
      var errorMsg = await response.Content.ReadAsStringAsync();
      throw new Exception("Error calling Groq API: " + errorMsg);
    }

      var jsonResponse = await response.Content.ReadAsStringAsync();


      using var doc = JsonDocument.Parse(jsonResponse);
      if (!doc.RootElement.TryGetProperty("choices", out var choices))
        {
            throw new Exception($"Groq API response mein 'choices' nahi mila. Raw Response: {jsonResponse}");
        }

     return choices[0]
                  .GetProperty("message")
                  .GetProperty("content")
                  .GetString() ?? string.Empty;
    
  }



  }
  