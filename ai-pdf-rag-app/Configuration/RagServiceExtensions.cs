using AiPdfRagApp.Services;
using AiPdfRagApp.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Qdrant.Client;

namespace AiPdfRagApp.Configuration;

public static class RagServiceExtensions
{
    public static IServiceCollection AddRagServices(this IServiceCollection services, RagOptions? options = null)
    {
        options ??= new RagOptions();

        services.AddSingleton(options);
        services.AddSingleton<IPdfTextExtractor, PdfTextExtractor>();
        services.AddSingleton<PdfIngestionService>();
        services.AddSingleton<RegulationQueryService>();
        services.AddSingleton<IRegulationQueryService>(provider => provider.GetRequiredService<RegulationQueryService>());
        services.AddSingleton(_ => new QdrantClient(options.QdrantEndpoint));
        services.AddQdrantVectorStore();

#pragma warning disable SKEXP0070
        services.AddOllamaEmbeddingGenerator(options.EmbeddingModel, options.OllamaEndpoint);
        services.AddOllamaChatCompletion(options.ChatModel, options.OllamaEndpoint);
#pragma warning restore SKEXP0070

        return services;
    }
}
