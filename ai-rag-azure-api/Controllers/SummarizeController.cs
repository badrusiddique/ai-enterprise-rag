using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

public record SummarizeRequest(string Text);
public record SummarizeResponse(string Summary);

[ApiController]
[Route("api/[controller]")]
[Tags("Controllers/Summarize")]
public class SummarizeController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;

    public SummarizeController(Kernel kernel, IChatCompletionService chatCompletionService)
    {
        _kernel = kernel;
        _chatCompletionService = chatCompletionService;
    }

    [HttpPost]
    public async Task<ActionResult<SummarizeResponse>> SummarizeWithKernelAsync([FromBody] SummarizeRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Text is required.");
        }

        // This path runs through the Semantic Kernel function pipeline, so IFunctionInvocationFilter executes.
        var result = await _kernel.InvokePromptAsync(
            "You are a helpful assistant that summarizes text.\n\nPlease summarize the following text: {{$input}}",
            new KernelArguments { ["input"] = request.Text }
        );

        return Ok(new SummarizeResponse(result.ToString() ?? string.Empty));
    }

    [HttpPost("with-chat")]
    public async Task<ActionResult<SummarizeResponse>> SummarizeWithChatCompletionAsync([FromBody] SummarizeRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Text is required.");
        }

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("You are a helpful assistant that summarizes text.");
        chatHistory.AddUserMessage($"Please summarize the following text: {request.Text}");

        var result = await _chatCompletionService.GetChatMessageContentAsync(chatHistory);

        return Ok(new SummarizeResponse(result.Content ?? string.Empty));
    }
}
