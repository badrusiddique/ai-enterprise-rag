using System.Text;
using AiPdfRagApp.Interfaces;
using AiPdfRagApp.Services;
using AiPdfRagApp.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Qdrant.Client;

namespace AiPdfRagApp.Configuration;

public static class RagServiceExtensions
{
    public static IServiceCollection AddRagServices(this IServiceCollection services, RagOptions? ragOptions = null, LangfuseOptions? langfuseOptions = null)
    {
        ragOptions ??= new RagOptions();
        langfuseOptions ??= new LangfuseOptions();

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

        services.AddTelemetry(langfuseOptions);

        return services;
    }

    private static IServiceCollection AddTelemetry(this IServiceCollection services, LangfuseOptions langfuseOptions)
    {
        if (!langfuseOptions.IsConfigured)
        {
            return services;
        }

        AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{langfuseOptions.PublicKey}:{langfuseOptions.SecretKey}"));

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("ai-pdf-rag-app"))
            .WithTracing(x => x
                .AddSource("Microsoft.SemanticKernel")
                .AddSource("Microsoft.SemanticKernel*")
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(langfuseOptions.TraceEndpoint);
                        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                        o.Headers = $"Authorization=Basic {credentials}";
                    }))
            .WithMetrics(m => m
                .AddMeter("Microsoft.SemanticKernel*")
                .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(langfuseOptions.MetricsEndpoint);
                        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                        o.Headers = $"Authorization=Basic {credentials}";
                    }));

        return services;
    }
}
