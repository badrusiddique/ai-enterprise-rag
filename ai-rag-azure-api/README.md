# AI RAG Azure API

This ASP.NET Core API demonstrates retrieval-augmented generation (RAG) and content safety with Azure services:

1. Azure OpenAI for chat and summarization.
2. Azure AI Search for vector-based document retrieval.
3. Azure Content Safety for prompt injection and content moderation.
4. Semantic Kernel filters for pre/post-invocation hooks.

## What the API does

1. `POST /api/summarize` — Summarizes text through Kernel function invocation (Semantic Kernel filters execute).
2. `POST /api/summarize/with-chat` — Summarizes text through direct chat service (filters do not execute).
3. `POST /api/chat` — Answers questions about a document corpus using vector search and RAG.
4. `POST /api/fileupload` — Uploads documents to Azure Blob Storage for ingestion into the knowledge base.
5. `GET /health` — Exposes health checks.

## Why there are two summarize endpoints

The two endpoints are useful for learning how Semantic Kernel filters behave:

1. `POST /api/summarize` runs through Kernel function invocation. `IFunctionInvocationFilter` implementations execute here.
2. `POST /api/summarize/with-chat` calls the connector directly. `IFunctionInvocationFilter` implementations do not execute here.

Current filter setup:

1. `StructuredLoggingFilter` logs function-level invocation details.
2. `ContentSafetyFilter` checks prompt and content safety before function execution using `IContentSafetyService`.
3. `SemanticCacheFilter` caches Kernel summarize responses for 10 minutes using normalized arguments as the cache key.

## Example requests

### Summarize via Kernel (filters run)

```http
POST http://localhost:5015/api/summarize
Content-Type: application/json

{
  "text": "Summarize this paragraph in two bullet points..."
}
```

### Summarize via Chat (filters do not run)

```http
POST http://localhost:5015/api/summarize/with-chat
Content-Type: application/json

{
  "text": "Summarize this paragraph in two bullet points..."
}
```

### Ask a RAG question

```http
POST http://localhost:5015/api/chat
Content-Type: application/json

{
  "message": "What does the regulation say about fall protection?"
}
```

### Upload a document

```http
POST http://localhost:5015/api/fileupload
Content-Type: multipart/form-data

[File: regulation.pdf]
```

## Project structure

```text
ai-rag-azure-api
  Configurations
    AzureOpenAIConfiguration.cs
    SemanticFilterConfiguration.cs
    TelemetryConfiguration.cs
  Controllers
    ChatController.cs
    FileUploadController.cs
    SummarizeController.cs
  Middlewares
    ContentSafetyFilter.cs
    SemanticCacheFilter.cs
    StructuredLoggingFilter.cs
  Services
    AzureAiSearchService.cs
    AzureContentSafetyService.cs
    AzureOpenAiService.cs
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
  "Azure": {
    "OpenAI": {
      "Endpoint": "https://<your-resource>.openai.azure.com",
      "ApiKey": "SET_LOCALLY",
      "Deployment": "gpt-4-mini"
    },
    "Search": {
      "Endpoint": "https://<your-resource>.search.windows.net",
      "IndexName": "regulation-index",
      "ApiKey": "SET_LOCALLY"
    },
    "ContentSafety": {
      "Endpoint": "https://<your-resource>.cognitiveservices.azure.com",
      "ApiKey": "SET_LOCALLY"
    },
    "BlobStorage": {
      "ConnectionString": "SET_LOCALLY",
      "ContainerName": "documents"
    }
  },
  "Langfuse": {
    "PublicKey": "SET_LOCALLY",
    "SecretKey": "SET_LOCALLY",
    "TraceEndpoint": "https://us.cloud.langfuse.com/api/public/otel/v1/traces",
    "MetricsEndpoint": "https://us.cloud.langfuse.com/api/public/otel/v1/metrics"
  }
}
```

If Azure Search settings are missing, chat endpoints will fail at runtime.

If Azure Blob Storage settings are missing, file upload will fail at runtime.

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
