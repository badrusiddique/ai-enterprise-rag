using System.Diagnostics;
using Microsoft.SemanticKernel;
using OpenAI.Chat;

public class StructuredLoggingFilter : IFunctionInvocationFilter
{
    private readonly ILogger<StructuredLoggingFilter> _logger;

    public StructuredLoggingFilter(ILogger<StructuredLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Stopwatch stopWatch = new();
        var (function, arguments) = (context.Function, context.Arguments);

        _logger.LogInformation("StructuredLoggingFilter: Invoking function '{FunctionName}' | plugin {Plugin} | input arguments {InputArgs}", function.Name, function.PluginName, string.Join(", ", arguments.Select(kvp => $"{kvp.Key}={kvp.Value}")));

        try
        {
            stopWatch.Start();
            await next(context);
            stopWatch.Stop();

            var usage = context.Result.Metadata?.GetValueOrDefault("Usage");
            if (usage is not null)
            {
                var chatTokenUsage = usage as ChatTokenUsage;
                _logger.LogInformation("StructuredLoggingFilter: Completed function '{FunctionName}' | plugin {Plugin} | elapsed time {ElapsedMilliseconds} ms | input tokens {InputTokens} | output tokens {OutputTokens}", function.Name, function.PluginName, stopWatch.ElapsedMilliseconds, chatTokenUsage?.InputTokenCount, chatTokenUsage?.OutputTokenCount);
            }
            else
            {
                _logger.LogInformation("StructuredLoggingFilter: Completed function '{FunctionName}' | plugin {Plugin} | elapsed time {ElapsedMilliseconds} ms | no usage information available", function.Name, function.PluginName, stopWatch.ElapsedMilliseconds);
            }

        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            _logger.LogError(ex, "StructuredLoggingFilter: Failed function '{FunctionName}' | plugin {Plugin} | elapsed time {ElapsedMilliseconds} ms", function.Name, function.PluginName, stopWatch.ElapsedMilliseconds);
            throw;
        }
    }
}
