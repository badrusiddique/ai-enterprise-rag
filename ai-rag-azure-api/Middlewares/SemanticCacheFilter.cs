using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;

public class SemanticCacheFilter : IFunctionInvocationFilter
{
    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    private readonly IMemoryCache _memoryCache;

    public SemanticCacheFilter(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var (function, arguments) = (context.Function, context.Arguments);
        var orderedArguments = arguments
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .Select(kvp => $"{kvp.Key}={kvp.Value}");

        var joinedArguments = string.Join("|", orderedArguments);
        var cacheKey = $"scf:{joinedArguments}";

        if (_memoryCache.TryGetValue(cacheKey, out string? cachedResult))
        {
            context.Result = new FunctionResult(function, cachedResult);
            return;
        }

        await next(context);

        var resultText = context.Result.ToString() ?? string.Empty;
        _memoryCache.Set(cacheKey, resultText, CacheOptions);
    }
}
