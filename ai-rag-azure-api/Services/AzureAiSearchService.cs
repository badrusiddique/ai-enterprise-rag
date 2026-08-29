
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

public interface IAzureAiSearchService
{
    Task<IList<string>> SearchChunksAsync(string query, int top = 5);
}

public class AzureAiSearchService : IAzureAiSearchService
{
    private readonly SearchClient _azureSearchClient;

    public AzureAiSearchService(IConfiguration configuration)
    {
        var endpoint = configuration["AzureSearch:Endpoint"];
        var indexName = configuration["AzureSearch:IndexName"];
        var apiKey = configuration["AzureSearch:ApiKey"];

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(indexName) || string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("AzureSearch configuration is missing Endpoint, IndexName, or ApiKey.");
        }

        _azureSearchClient = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));
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
