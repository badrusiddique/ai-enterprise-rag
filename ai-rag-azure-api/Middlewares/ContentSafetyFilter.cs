using Microsoft.SemanticKernel;

public class ContentSafetyFilter : IFunctionInvocationFilter
{
    private static readonly string[] UnsafeKeywords = ["violence", "hate", "illegal"];

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
            }
        }
        await next(context);
    }

    private Task<bool> IsContentSafeAsync(string content)
    {
        // Placeholder behavior keeps requests flowing until a real safety check is wired in.
        return Task.FromResult(!UnsafeKeywords.Any(keyword => content.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
    }
}
