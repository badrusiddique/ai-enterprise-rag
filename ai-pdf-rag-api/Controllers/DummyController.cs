using Microsoft.AspNetCore.Mvc;

namespace AiPdfRagApi.Controllers;

[ApiController]
[Route("api/dummy")]
[Tags("Controllers/Dummy")]
public class DummyController : ControllerBase
{
    #region Public methods

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("GET request successful.");
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok("POST request successful.");
    }

    [HttpPut]
    public IActionResult Put()
    {
        return Ok("PUT request successful.");
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        return Ok("DELETE request successful.");
    }

    #endregion
}
