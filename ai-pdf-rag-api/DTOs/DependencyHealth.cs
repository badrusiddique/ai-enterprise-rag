namespace AiPdfRagApi.DTOs;

public record DependencyHealthResponseDto(
    DependencyHealthDto Qdrant,
    DependencyHealthDto Ollama);

public record DependencyHealthDto(
    string Name,
    Uri Url,
    bool IsAvailable,
    int? StatusCode);
