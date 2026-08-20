namespace AiPdfRagApp.Configuration;

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
