
using Microsoft.SemanticKernel;

public sealed class AzureOpenAIOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Deployment { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Endpoint)
        && !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(Deployment);
}

public static class AzureOpenAIConfiguration
{
    public static IServiceCollection AddAzureOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var azureOpenAIOptions = configuration.GetSection("Azure:OpenAI").Get<AzureOpenAIOptions>() ?? new AzureOpenAIOptions();

        if (!azureOpenAIOptions.IsConfigured)
        {
            return services;
        }

        services
            .AddKernel()
            .AddAzureOpenAIChatCompletion(
                deploymentName: azureOpenAIOptions.Deployment,
                endpoint: azureOpenAIOptions.Endpoint,
                apiKey: azureOpenAIOptions.ApiKey);

        return services;
    }
}
