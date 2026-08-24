using System.Text.Json;
using Azure;
using Azure.AI.ContentSafety;

public interface IContentSafetyService
{
    Task<bool> IsContentSafeAsync(string content);
    Task<bool> IsPromptSafeAsync(string userPrompt);
}

public class AzureContentSafetyService : IContentSafetyService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ContentSafetyClient _contentSafetyClient;

    public AzureContentSafetyService(IConfiguration configuration, HttpClient httpClient)
    {
        var apiKey = configuration["AzureContentSafety:ApiKey"];
        var endpoint = configuration["AzureContentSafety:Endpoint"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("AzureContentSafety configuration is missing ApiKey or Endpoint.");
        }

        _httpClient = httpClient;
        _configuration = configuration;
        _contentSafetyClient = new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    /// <summary>
    /// AnalyzeText — detects harmful content categories: Hate, Violence, Sexual, SelfHarm.
    /// It looks at the content itself and asks "is this text harmful?"
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public async Task<bool> IsContentSafeAsync(string content)
    {
        var options = new AnalyzeTextOptions(content);
        var response = await _contentSafetyClient.AnalyzeTextAsync(options);

        foreach (var category in response.Value.CategoriesAnalysis)
        {
            if (category.Severity >= 2)
            {
                return false; // Content is unsafe
            }
        }

        return true; // Content is safe
    }

    /// <summary>
    /// shieldPrompt — detects adversarial attacks on the AI system itself: prompt injection and jailbreak attempts.
    /// It looks at the intent and asks "is this trying to hijack the application's behaviour?"
    /// </summary>
    /// <param name="userPrompt"></param>
    /// <returns></returns>
    public async Task<bool> IsPromptSafeAsync(string userPrompt)
    {
        var sheildPromptUrl = $"{_configuration["AzureContentSafety:Endpoint"]}/contentsafety/text:shieldPrompt?api-version=2024-09-01";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, sheildPromptUrl);
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _configuration["AzureContentSafety:ApiKey"]);
        httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(new { userPrompt }), System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(httpRequestMessage);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ShieldPromptResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.UserPromptAnalysis?.AttackDetected == false;
    }
}

public record UserPromptAnalysis(bool AttackDetected);
public record ShieldPromptResponse(UserPromptAnalysis? UserPromptAnalysis);
