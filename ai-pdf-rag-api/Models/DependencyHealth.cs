namespace AiPdfRagApi.Models;

public record DependencyHealth(
    string Name,
    Uri Url,
    bool IsAvailable,
    int? StatusCode);
