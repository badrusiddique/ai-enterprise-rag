using Microsoft.SemanticKernel;

public class ContentSafetyFilter : IFunctionInvocationFilter
{
    private readonly IContentSafetyService _contentSafetyService;

    public ContentSafetyFilter(IContentSafetyService contentSafetyService)
    {
        _contentSafetyService = contentSafetyService;
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var (function, arguments) = (context.Function, context.Arguments);

        foreach (var argument in arguments)
        {
            if (argument.Value is string content)
            {
                if (!await IsContentSafeAsync(content))
                {
                    context.Result = new FunctionResult(function, "Content is not safe, I cannot process this request.");
                    return;
                }

                if (!await IsPromptSafeAsync(content))
                {
                    context.Result = new FunctionResult(function, "Prompt Injection detected, I cannot process this request.");
                    return;
                }
            }
        }

        await next(context);
    }

    private Task<bool> IsContentSafeAsync(string content)
    {
        return _contentSafetyService.IsContentSafeAsync(content);
    }

    private Task<bool> IsPromptSafeAsync(string userPrompt)
    {
        return _contentSafetyService.IsPromptSafeAsync(userPrompt);
    }
}
