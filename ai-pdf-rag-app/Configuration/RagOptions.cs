namespace AiPdfRagApp.Configuration;

public sealed class RagOptions
{
    public Uri QdrantHttpEndpoint { get; } = new("http://localhost:6333");
    public Uri QdrantEndpoint { get; } = new("http://localhost:6334");
    public Uri OllamaEndpoint { get; } = new("http://localhost:11434");
    public string CollectionName { get; } = "00-ohs-regulations";
    public string EmbeddingModel { get; } = "nomic-embed-text:latest";
    public string ChatModel { get; } = "gemma3:latest";
    public string PdfPath { get; } = Path.Combine(
        AppContext.BaseDirectory,
        "Corpus",
        "bc-ohs-regulation-parts-10-11-2026.pdf");
    public int MaximumTokensPerLine { get; } = 100;
    public int MaximumTokensPerChunk { get; } = 512;
    public int OverlapTokens { get; } = 50;
    public int SearchResultCount { get; } = 3;
}
