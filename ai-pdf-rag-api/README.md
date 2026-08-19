# AI PDF RAG API

This ASP.NET Core API exposes the PDF regulation RAG query flow over HTTP. It reuses the services from `ai-pdf-rag-app` through a project reference, so the console app and API share the same Qdrant, Ollama, ingestion, retrieval, and chat configuration.

## What the API does

1. Accepts a regulation question at `POST /api/regulation/query`.
2. Validates that the request contains a non-empty query.
3. Calls `IRegulationQueryService.AskAsync` from `ai-pdf-rag-app`.
4. Returns the generated answer and retrieved references as JSON.
5. Checks local Qdrant and Ollama availability at `GET /api/health/dependencies`.

## Request

```http
POST http://localhost:5053/api/regulation/query
Content-Type: application/json

{
  "query": "When is fall protection required?"
}
```

## Response

```json
{
  "answer": "Use fall protection when required.",
  "references": [
    "Score: 82.10 % Page: 10 Content: ..."
  ]
}
```

## Dependency health

```http
GET http://localhost:5053/api/health/dependencies
Accept: application/json
```

The health endpoint checks the Qdrant HTTP endpoint and Ollama tags endpoint configured in `RagOptions`. It does not query the vector collection or run a model prompt.

## Project structure

```text
ai-pdf-rag-api
  Controllers
    DummyController.cs
    HealthController.cs
    RegulationController.cs
  DTOs
    DependencyHealth.cs
    Regulation.cs
  Properties
    launchSettings.json
  Program.cs
```

`Program.cs` registers normal API services and calls `AddRagServices()` from `ai-pdf-rag-app`. `RegulationController.cs` stays thin: it validates the request and delegates the RAG work to the shared query service. `HealthController.cs` only checks whether Qdrant and Ollama respond at their configured URLs.

## Prerequisites

Use the same local services and models as `ai-pdf-rag-app`:

1. Qdrant on ports `6333` and `6334`.
2. Ollama on port `11434`.
3. The `nomic-embed-text` and `gemma3` models.
4. An indexed Qdrant collection. Run `ai-pdf-rag-app` once if the collection has not been created yet.

## Run the API

From the repository root:

```bash
dotnet run --project ai-pdf-rag-api/ai-pdf-rag-api.csproj
```

The development profile listens on:

```text
http://localhost:5053
https://localhost:7188
```

Scalar API documentation is available in Development at:

```text
/scalar/v1
```

To debug in VS Code, choose `C#: PDF RAG API` from the Run and Debug menu. The profile builds the API first and opens Scalar when the server starts.

## Verify

```bash
dotnet build ai-pdf-rag-api/ai-pdf-rag-api.csproj --configuration Debug
dotnet test ai-pdf-rag-api.Tests/ai-pdf-rag-api.Tests.csproj --configuration Debug
```
