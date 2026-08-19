namespace AiPdfRagApp.Configuration;

public sealed class RagOptions
{
    public Uri QdrantHttpEndpoint { get; set; } = new("http://localhost:6333");
    public Uri QdrantEndpoint { get; set; } = new("http://localhost:6334");
    public Uri OllamaEndpoint { get; set; } = new("http://localhost:11434");
    public string CollectionName { get; set; } = "00-ohs-regulations";
    public string EmbeddingModel { get; set; } = "nomic-embed-text:latest";
    public string ChatModel { get; set; } = "gemma3:latest";
    public string PdfPath { get; set; } = Path.Combine(
        AppContext.BaseDirectory,
        "Corpus",
        "bc-ohs-regulation-parts-10-11-2026.pdf");
    public int MaximumTokensPerLine { get; set; } = 100;
    public int MaximumTokensPerChunk { get; set; } = 512;
    public int OverlapTokens { get; set; } = 50;
    public int SearchResultCount { get; set; } = 3;
}
