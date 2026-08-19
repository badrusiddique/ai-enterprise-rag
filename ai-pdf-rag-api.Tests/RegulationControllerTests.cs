using AiPdfRagApi.Controllers;
using AiPdfRagApi.DTOs;
using AiPdfRagApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AiPdfRagApi.Tests;

public sealed class RegulationControllerTests
{
    [Fact]
    public void Tags_GroupRegulationUnderControllers()
    {
        var tags = typeof(RegulationController)
            .GetCustomAttributes(typeof(TagsAttribute), inherit: false)
            .Cast<TagsAttribute>()
            .Single();

        Assert.Equal(["Controllers/Regulation"], tags.Tags);
    }

    [Fact]
    public async Task QueryRegulation_WhenQueryIsBlank_ReturnsBadRequest()
    {
        var controller = new RegulationController(new FakeRegulationQueryService());

        var result = await controller.QueryRegulation(new RegulationRequestDto(" "));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Query is required.", badRequest.Value);
    }

    [Fact]
    public async Task QueryRegulation_WhenQueryIsProvided_ReturnsAnswerAndReferences()
    {
        var queryService = new FakeRegulationQueryService
        {
            Answer = "Use fall protection when required.",
            References = ["Page: 10 Content: Fall protection details"]
        };
        var controller = new RegulationController(queryService);

        var result = await controller.QueryRegulation(new RegulationRequestDto("When is fall protection required?"));

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<RegulationResponseDto>(okResult.Value);
        Assert.Equal("Use fall protection when required.", response.Answer);
        Assert.Equal(queryService.References, response.References);
        Assert.Equal("When is fall protection required?", queryService.Question);
    }

    private sealed class FakeRegulationQueryService : IRegulationQueryService
    {
        public string? Question { get; private set; }
        public string Answer { get; init; } = string.Empty;
        public IReadOnlyList<string> References { get; init; } = [];

        public Task<(string Answer, IReadOnlyList<string> References)> AskAsync(string question)
        {
            Question = question;

            return Task.FromResult((Answer, References));
        }
    }
}
