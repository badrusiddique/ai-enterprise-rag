using AiPdfRagApi.DTOs;
using AiPdfRagApp.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace AiPdfRagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(HttpClient httpClient, RagOptions options) : ControllerBase
{
    [HttpGet("dependencies")]
    public async Task<ActionResult<DependencyHealthResponseDto>> GetDependencies(CancellationToken cancellationToken = default)
    {
        var qdrant = await CheckAsync("Qdrant", options.QdrantHttpEndpoint, cancellationToken);
        var ollama = await CheckAsync("Ollama", new Uri(options.OllamaEndpoint, "/api/tags"), cancellationToken);

        return Ok(new DependencyHealthResponseDto(qdrant, ollama));
    }

    private async Task<DependencyHealthDto> CheckAsync(string name, Uri url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return new DependencyHealthDto(name, url, response.IsSuccessStatusCode, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return new DependencyHealthDto(name, url, false, null);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new DependencyHealthDto(name, url, false, null);
        }
    }
}
