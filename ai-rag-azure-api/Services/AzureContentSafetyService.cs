using System.Text.Json;
using Azure;
using Azure.AI.ContentSafety;
using Microsoft.Extensions.Options;

public interface IContentSafetyService
{
    Task<bool> IsContentSafeAsync(string content);
    Task<bool> IsPromptSafeAsync(string userPrompt);
}

public sealed class AzureContentSafetyOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Endpoint)
        && !string.IsNullOrWhiteSpace(ApiKey);
}

public class AzureContentSafetyService : IContentSafetyService
{
    private readonly HttpClient _httpClient;
    private readonly AzureContentSafetyOptions _options;
    private readonly ContentSafetyClient _contentSafetyClient;

    public AzureContentSafetyService(IOptions<AzureContentSafetyOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        var apiKey = _options.ApiKey;
        var endpoint = _options.Endpoint;

        if (!_options.IsConfigured)
        {
            throw new InvalidOperationException("Azure ContentSafety configuration is missing Endpoint or ApiKey.");
        }

        _httpClient = httpClient;
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
        var sheildPromptUrl = $"{_options.Endpoint}/contentsafety/text:shieldPrompt?api-version=2024-09-01";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, sheildPromptUrl);
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _options.ApiKey);
        httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(new { userPrompt }), System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(httpRequestMessage);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ShieldPromptResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.UserPromptAnalysis?.AttackDetected == false;
    }
}

public record UserPromptAnalysis(bool AttackDetected);
public record ShieldPromptResponse(UserPromptAnalysis? UserPromptAnalysis);
