using AiPdfRagApi.DTOs;
using AiPdfRagApi.Models;
using AiPdfRagApp.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiPdfRagApi.Controllers;

[ApiController]
[Route("api/health")]
[Tags("Controllers/Health")]
public class HealthController(HttpClient httpClient, RagOptions options) : ControllerBase
{
    #region Public methods

    [HttpGet]
    public async Task<ActionResult<HealthResponseDto>> GetDependencies(CancellationToken cancellationToken = default)
    {
        var qdrant = await CheckAsync("Qdrant", options.QdrantHttpEndpoint, cancellationToken);
        var ollama = await CheckAsync("Ollama", new Uri(options.OllamaEndpoint, "/api/tags"), cancellationToken);

        return Ok(new HealthResponseDto(
            qdrant.Url,
            qdrant.IsAvailable,
            qdrant.StatusCode,
            ollama.Url,
            ollama.IsAvailable,
            ollama.StatusCode));
    }

    #endregion

    #region Private methods

    private async Task<DependencyHealth> CheckAsync(string name, Uri url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return new DependencyHealth(name, url, response.IsSuccessStatusCode, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return new DependencyHealth(name, url, false, null);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new DependencyHealth(name, url, false, null);
        }
    }

    #endregion
}
