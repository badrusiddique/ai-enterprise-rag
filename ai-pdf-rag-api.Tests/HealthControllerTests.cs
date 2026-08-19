using AiPdfRagApi.Controllers;
using AiPdfRagApi.DTOs;
using AiPdfRagApi.Models;
using AiPdfRagApp.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace AiPdfRagApi.Tests;

public sealed class HealthControllerTests
{
    [Fact]
    public void Route_UsesLowercaseHealthPath()
    {
        var route = typeof(HealthController)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Single();

        var httpGet = typeof(HealthController)
            .GetMethod(nameof(HealthController.GetDependencies))!
            .GetCustomAttributes(typeof(HttpGetAttribute), inherit: false)
            .Cast<HttpGetAttribute>()
            .Single();

        Assert.Equal("api/health", route.Template);
        Assert.Null(httpGet.Template);
    }

    [Fact]
    public void Tags_GroupHealthUnderControllers()
    {
        var tags = typeof(HealthController)
            .GetCustomAttributes(typeof(TagsAttribute), inherit: false)
            .Cast<TagsAttribute>()
            .Single();

        Assert.Equal(["Controllers/Health"], tags.Tags);
    }

    [Fact]
    public void HealthResponse_UsesDtoForApiOutputAndModelForInternalState()
    {
        Assert.Equal("AiPdfRagApi.DTOs", typeof(HealthResponseDto).Namespace);
        Assert.Equal("AiPdfRagApi.Models", typeof(DependencyHealth).Namespace);
    }

    [Fact]
    public async Task GetDependencies_WhenQdrantAndOllamaRespond_ReturnsAvailableStatuses()
    {
        var controller = new HealthController(
            new HttpClient(new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK))),
            new RagOptions());

        var result = await controller.GetDependencies();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HealthResponseDto>(okResult.Value);
        Assert.True(response.IsQdrantAvailable);
        Assert.True(response.IsOllamaAvailable);
    }

    [Fact]
    public async Task GetDependencies_WhenServiceDoesNotRespond_ReturnsUnavailableStatus()
    {
        var controller = new HealthController(
            new HttpClient(new StubHttpMessageHandler(_ => throw new HttpRequestException("Connection refused"))),
            new RagOptions());

        var result = await controller.GetDependencies();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HealthResponseDto>(okResult.Value);
        Assert.False(response.IsQdrantAvailable);
        Assert.False(response.IsOllamaAvailable);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(responseFactory(request));
        }
    }
}
