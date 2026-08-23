using System.Text;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public sealed class LangfuseOptions
{
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string TraceEndpoint { get; set; } = string.Empty;
    public string MetricsEndpoint { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(PublicKey)
        && !string.IsNullOrWhiteSpace(SecretKey)
        && !string.IsNullOrWhiteSpace(TraceEndpoint)
        && !string.IsNullOrWhiteSpace(MetricsEndpoint);
}

public static class TelemetryConfiguration
{
    public static IServiceCollection AddTelemetryConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var langfuseOptions = configuration.GetSection("Langfuse").Get<LangfuseOptions>() ?? new LangfuseOptions();

        if (!langfuseOptions.IsConfigured)
        {
            return services;
        }

        AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{langfuseOptions.PublicKey}:{langfuseOptions.SecretKey}"));

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("ai-rag-azure-api"))
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
