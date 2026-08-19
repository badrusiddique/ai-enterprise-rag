using AiPdfRagApi.Controllers;
using AiPdfRagApi.DTOs;
using AiPdfRagApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace AiPdfRagApi.Tests;

public sealed class HealthControllerTests
{
    [Fact]
    public async Task GetDependencies_WhenQdrantAndOllamaRespond_ReturnsAvailableStatuses()
    {
        var controller = new HealthController(
            new HttpClient(new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK))),
            new RagOptions());

        var result = await controller.GetDependencies();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<DependencyHealthResponseDto>(okResult.Value);
        Assert.True(response.Qdrant.IsAvailable);
        Assert.True(response.Ollama.IsAvailable);
    }

    [Fact]
    public async Task GetDependencies_WhenServiceDoesNotRespond_ReturnsUnavailableStatus()
    {
        var controller = new HealthController(
            new HttpClient(new StubHttpMessageHandler(_ => throw new HttpRequestException("Connection refused"))),
            new RagOptions());

        var result = await controller.GetDependencies();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<DependencyHealthResponseDto>(okResult.Value);
        Assert.False(response.Qdrant.IsAvailable);
        Assert.False(response.Ollama.IsAvailable);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(responseFactory(request));
        }
    }
}
