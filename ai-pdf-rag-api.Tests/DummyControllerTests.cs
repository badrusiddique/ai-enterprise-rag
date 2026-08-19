using AiPdfRagApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AiPdfRagApi.Tests;

public sealed class DummyControllerTests
{
    [Fact]
    public void Route_UsesLowercaseDummyPath()
    {
        var route = typeof(DummyController)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Single();

        Assert.Equal("api/dummy", route.Template);
    }

    [Fact]
    public void Tags_GroupDummyUnderControllers()
    {
        var tags = typeof(DummyController)
            .GetCustomAttributes(typeof(TagsAttribute), inherit: false)
            .Cast<TagsAttribute>()
            .Single();

        Assert.Equal(["Controllers/Dummy"], tags.Tags);
    }

    [Fact]
    public void Get_ReturnsOk()
    {
        var controller = new DummyController();

        var result = controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("GET request successful.", okResult.Value);
    }

    [Fact]
    public void Post_ReturnsOk()
    {
        var controller = new DummyController();

        var result = controller.Post();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("POST request successful.", okResult.Value);
    }

    [Fact]
    public void Put_ReturnsOk()
    {
        var controller = new DummyController();

        var result = controller.Put();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("PUT request successful.", okResult.Value);
    }

    [Fact]
    public void Delete_ReturnsOk()
    {
        var controller = new DummyController();

        var result = controller.Delete();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("DELETE request successful.", okResult.Value);
    }
}
