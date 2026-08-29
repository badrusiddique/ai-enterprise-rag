
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Tags("Controllers/Chat")]
public class ChatController : ControllerBase
{
    private readonly IAzureAiSearchService _azureAiSearchService;
    private readonly IAzureOpenAiService _azureOpenAiService;

    public ChatController(IAzureAiSearchService azureAiSearchService, IAzureOpenAiService azureOpenAiService)
    {
        _azureOpenAiService = azureOpenAiService;
        _azureAiSearchService = azureAiSearchService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be null or empty.");
        }

        var searchResults = await _azureAiSearchService.SearchChunksAsync(request.Message);
        var queryResponse = await _azureOpenAiService.AskAnythingAsync(searchResults, request.Message);

        return Ok(new ChatResponse(queryResponse));
    }
}

public record ChatRequest(string Message);
public record ChatResponse(string Response);
