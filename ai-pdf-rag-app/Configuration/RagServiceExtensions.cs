using AiPdfRagApp.Interfaces;
using AiPdfRagApp.Services;
using AiPdfRagApp.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Qdrant.Client;

namespace AiPdfRagApp.Configuration;

public static class RagServiceExtensions
{
    public static IServiceCollection AddRagServices(this IServiceCollection services, RagOptions? ragOptions = null)
    {
        ragOptions ??= new RagOptions();

        services.AddSingleton(ragOptions);
        services.AddSingleton<IPdfTextExtractor, PdfTextExtractor>();
        services.AddSingleton<PdfIngestionService>();
        services.AddSingleton<IRegulationQueryService, RegulationQueryService>();
        services.AddSingleton(_ => new QdrantClient(ragOptions.QdrantEndpoint));
        services.AddQdrantVectorStore();

#pragma warning disable SKEXP0070
        services.AddOllamaEmbeddingGenerator(ragOptions.EmbeddingModel, ragOptions.OllamaEndpoint);
        services.AddOllamaChatCompletion(ragOptions.ChatModel, ragOptions.OllamaEndpoint);
#pragma warning restore SKEXP0070

        return services;
    }
}
