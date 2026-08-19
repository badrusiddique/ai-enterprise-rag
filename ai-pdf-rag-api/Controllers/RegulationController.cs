using AiPdfRagApi.DTOs;
using AiPdfRagApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiPdfRagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegulationController : ControllerBase
{
    private readonly IRegulationQueryService _regulationQueryService;

    public RegulationController(IRegulationQueryService regulationQueryService)
    {
        _regulationQueryService = regulationQueryService;
    }

    [HttpPost("query")]
    public async Task<ActionResult<RegulationResponseDto>> QueryRegulation([FromBody] RegulationRequestDto requestDto)
    {
        if (requestDto == null || string.IsNullOrWhiteSpace(requestDto.Query))
        {
            return BadRequest("Query is required.");
        }

        var (answer, references) = await _regulationQueryService.AskAsync(requestDto.Query);

        return Ok(new RegulationResponseDto(answer, references));
    }
}
