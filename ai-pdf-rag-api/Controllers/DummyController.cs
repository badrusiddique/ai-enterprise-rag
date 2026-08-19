using Microsoft.AspNetCore.Mvc;

namespace AiPdfRagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DummyController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API is running.");
    }
}
