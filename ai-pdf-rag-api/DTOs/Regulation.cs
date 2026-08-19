namespace AiPdfRagApi.DTOs;

public record RegulationRequestDto(string Query);

public record RegulationResponseDto(string Answer, IReadOnlyList<string> References);
