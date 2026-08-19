# AI PDF RAG app

This console application answers questions about selected parts of the British Columbia Occupational Health and Safety Regulation. It runs locally with Ollama for embeddings and chat completion, Qdrant for vector search, and PdfPig for PDF text extraction.

The project is intentionally small. It shows the complete RAG path without hiding the important steps behind a large framework.

## What the application does

1. Reads the regulation PDF one page at a time.
2. Splits each page into lines and overlapping text chunks.
3. Creates a 768 value embedding for each chunk with `nomic-embed-text`.
4. Stores the embedding, page number, and source text in Qdrant.
5. Embeds each question and retrieves the three closest chunks.
6. Sends only the retrieved regulation text and the question to `gemma3`.
7. Prints the answer with the retrieved pages and similarity scores.

The Qdrant collection is created on the first run. Later runs reuse it, so the PDF is not embedded every time the application starts. If first run ingestion fails, the incomplete collection is removed so the next run can start cleanly.

## Project structure

```text
ai-pdf-rag-app
  Configuration
    RagOptions.cs
    RagServiceExtensions.cs
  Interfaces
    IRegulationQueryService.cs
  Models
    OhsRegulation.cs
  Services
    PdfIngestionService.cs
    RegulationQueryService.cs
  Corpus
    bc-ohs-regulation-parts-10-11-2026.pdf
    README.md
  Docs
    Decisions
      week-01-chunking-and-rigidity.md
  Utilities
    PdfTextExtractor.cs
  Program.cs
```

`Program.cs` is the console entry point. It uses `Configuration/RagServiceExtensions.cs` to register dependencies, ensures the PDF is indexed, and passes each question to the query service. The API project uses the same extension method so the RAG service setup stays in one place.

`Services/PdfIngestionService.cs` owns the path from PDF pages to chunks, embeddings, and Qdrant records. `Services/RegulationQueryService.cs` owns retrieval, prompt construction, and conversation history. These are the two substantial operations in the application.

`Configuration/RagOptions.cs` keeps the values used by more than one file in one visible place. `Models/OhsRegulation.cs` declares the record stored in Qdrant. It has one key, two payload fields, and the vector definition expected by the embedding model.

`Utilities/PdfTextExtractor.cs` contains the small PdfPig boundary. Its name describes the one operation it performs, and keeping extraction separate makes `Program.cs` easier to scan without introducing another application layer.

The ingestion service is registered directly. The query service also implements `Interfaces/IRegulationQueryService.cs` because the API project consumes that boundary and its controller tests should not start Ollama or Qdrant. Repositories and extra result classes are omitted until the application has a real need for them.

## Prerequisites

1. .NET 9 SDK or a later SDK that can target .NET 9.
2. Docker or another way to run Qdrant.
3. Ollama running locally.
4. The `nomic-embed-text` and `gemma3` models.

Pull the models:

```bash
ollama pull nomic-embed-text
ollama pull gemma3
```

Start Qdrant with its HTTP and gRPC ports:

```bash
docker run --rm --name ai-rag-qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant
```

## Run the application

From the repository root:

```bash
dotnet run --project ai-pdf-rag-app/ai-pdf-rag-app.csproj
```

The PDF is copied into the build output, so the command works from the repository root or the project folder.

Ask a question such as:

```text
When is fall protection required?
```

Type `exit` to end the session.

To debug this project in VS Code, choose `C#: PDF RAG app` from the Run and Debug menu. The profile builds the Debug target first and then starts the executable. Qdrant and Ollama must be running for ingestion and questions to work.

## Configuration

The local endpoints, model names, collection name, chunk sizes, and result count are in `Configuration/RagOptions.cs`. The options object is registered once and injected into the two services.

## Collection changes

The vector schema is tied to the embedding model. `nomic-embed-text` produces 768 values, and `OhsRegulation` declares the same dimension.

Delete and rebuild the Qdrant collection after changing the embedding model, vector dimension, distance function, chunking values, or source PDF. Reusing an old collection after one of those changes can produce stale results or a schema error.

The local Qdrant dashboard is available at `http://localhost:6333/dashboard`.

## Current limits

1. The Semantic Kernel Ollama and Qdrant packages are alpha and preview packages. Their APIs can change between versions.
2. PdfPig exposes page text, but reading order can be imperfect in PDFs with complex columns or positioned text.
3. Ingestion writes one vector at a time. This keeps the learning flow readable, but batching would be faster for a large corpus.
4. Conversation history lasts only for the current process. The chat history keeps a maximum of five messages, including the system prompt. After each response, that leaves the latest four conversation messages, or two complete exchanges.
5. This is a learning project and not a source of legal advice.

## Documentation basis

The implementation was checked against the installed package metadata and the Context7 library pages for [Semantic Kernel](https://context7.com/microsoft/semantic-kernel), [Qdrant .NET](https://context7.com/qdrant/qdrant-dotnet), and [PdfPig](https://context7.com/uglytoad/pdfpig). The installed versions remain the source of truth for the code in this repository.

The chunking and schema tradeoffs are recorded in [week 01 chunking and rigidity](Docs/Decisions/week-01-chunking-and-rigidity.md).
