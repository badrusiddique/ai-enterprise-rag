
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;

public sealed class AzureSearchOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string IndexName { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Endpoint)
        && !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(IndexName);
}

public interface IAzureAiSearchService
{
    Task<IList<string>> SearchChunksAsync(string query, int top = 5);
}

public class AzureAiSearchService : IAzureAiSearchService
{
    private readonly SearchClient _azureSearchClient;

    public AzureAiSearchService(IOptions<AzureSearchOptions> options)
    {
        var settings = options.Value;

        if (!settings.IsConfigured)
        {
            throw new InvalidOperationException("Azure Search configuration is missing Endpoint, IndexName, or ApiKey.");
        }

        _azureSearchClient = new SearchClient(new Uri(settings.Endpoint), settings.IndexName, new AzureKeyCredential(settings.ApiKey));
    }


    public async Task<IList<string>> SearchChunksAsync(string query, int top = 5)
    {
        var searchOptions = new SearchOptions { IncludeTotalCount = true, Size = top };
        searchOptions.Select.Add("chunk");

        var response = await _azureSearchClient.SearchAsync<SearchDocument>(query, searchOptions);

        return response.Value.GetResults()
            .Select(v => v.Document.TryGetValue("chunk", out var chunk) ? chunk?.ToString() ?? string.Empty : string.Empty)
            .Where(c => !string.IsNullOrEmpty(c))
            .ToList();
    }
}
