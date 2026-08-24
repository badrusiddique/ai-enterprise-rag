using Microsoft.SemanticKernel;

public static class SemanticFilterConfiguration
{
    public static IServiceCollection AddSemanticFilterConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IFunctionInvocationFilter, StructuredLoggingFilter>();
        services.AddSingleton<IFunctionInvocationFilter, ContentSafetyFilter>();
        services.AddSingleton<IFunctionInvocationFilter, SemanticCacheFilter>();

        return services;
    }
}
