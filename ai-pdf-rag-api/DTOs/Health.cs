namespace AiPdfRagApi.DTOs;

public record HealthResponseDto(
    Uri QdrantUrl,
    bool IsQdrantAvailable,
    int? QdrantStatusCode,
    Uri OllamaUrl,
    bool IsOllamaAvailable,
    int? OllamaStatusCode);
