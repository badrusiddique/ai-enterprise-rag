# AI RAG Azure API

This ASP.NET Core API exposes Azure OpenAI summarization over HTTP and demonstrates two integration styles:

1. Summarization through Semantic Kernel function invocation.
2. Summarization through direct `IChatCompletionService` connector calls.

The project is intentionally small and focused so the differences between these two paths are easy to observe and debug.

## What the API does

1. Accepts text input at `POST /api/summarize` and summarizes through `Kernel.InvokePromptAsync`.
2. Accepts text input at `POST /api/summarize/with-chat` and summarizes through `IChatCompletionService.GetChatMessageContentAsync`.
3. Exposes health checks at `GET /health`.
4. Exposes OpenAPI and Scalar in Development.

## Why there are two summarize endpoints

The two endpoints are useful for learning how Semantic Kernel filters behave:

1. `POST /api/summarize` runs through Kernel function invocation. `IFunctionInvocationFilter` implementations execute here.
2. `POST /api/summarize/with-chat` calls the connector directly. `IFunctionInvocationFilter` implementations do not execute here.

## Example requests

### Kernel path

```http
POST http://localhost:5015/api/summarize
Content-Type: application/json

{
  "text": "Summarize this paragraph in two bullet points..."
}
```

### Direct chat connector path

```http
POST http://localhost:5015/api/summarize/with-chat
Content-Type: application/json

{
  "text": "Summarize this paragraph in two bullet points..."
}
```

## Project structure

```text
ai-rag-azure-api
  Configurations
    AzureOpenAIConfiguration.cs
    SemanticFilterConfiguration.cs
    TelemetryConfiguration.cs
  Controllers
    SummarizeController.cs
  Middlewares
    StructuredLoggingFilter.cs
  Properties
    launchSettings.json
  Program.cs
```

`Program.cs` wires API services, Semantic Kernel, optional telemetry, and endpoint mappings. `SummarizeController.cs` keeps the endpoint behavior explicit and validates input before invoking model calls.

## Configuration

The API reads normal ASP.NET configuration files and then optionally loads `appsettings.Local.json`.

1. Keep `appsettings.Development.json` safe to commit.
2. Put real secrets in `appsettings.Local.json`.
3. `appsettings.Local.json` is git-ignored.

Expected configuration shape:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://<your-resource>.openai.azure.com",
    "ApiKey": "SET_LOCALLY",
    "Deployment": "gpt-4.1-mini"
  },
  "Langfuse": {
    "PublicKey": "SET_LOCALLY",
    "SecretKey": "SET_LOCALLY",
    "TraceEndpoint": "https://us.cloud.langfuse.com/api/public/otel/v1/traces",
    "MetricsEndpoint": "https://us.cloud.langfuse.com/api/public/otel/v1/metrics"
  }
}
```

If Azure OpenAI settings are missing, chat services are not configured and summarize endpoints will fail at runtime.

If Langfuse settings are missing, telemetry export is skipped.

## Run

From repository root:

```bash
dotnet run --project ai-rag-azure-api/ai-rag-azure-api.csproj
```

Default development URLs from launch settings:

```text
http://localhost:5015
https://localhost:7230
```

Scalar is available in Development at:

```text
/scalar/v1
```

OpenAPI document is available at:

```text
/openapi/v1.json
```

## Debug in VS Code

Use the `C#: RAG Azure API` launch profile. The profile builds the project first and opens Scalar when the server reports the listening URL.

If startup fails with an address already in use error, another process is already bound to the same port.

## Verify

```bash
dotnet build ai-rag-azure-api/ai-rag-azure-api.csproj --configuration Debug
```
