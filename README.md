# AI enterprise RAG

This repository is a set of small projects for learning retrieval augmented generation with local models. Each project isolates one part of the problem so the code remains readable while the examples grow in capability.

## Projects

### [AI RAG movie app](./ai-rag-movie-app)

A base RAG pipeline built with `Microsoft.Extensions.AI`. It embeds a curated movie data set in Qdrant and answers questions from that data.

### [AI Semantic Kernel RAG movie app](./ai-semantic-rag-movie-app)

A Semantic Kernel version of the movie example. It adds plugins, function calling, tool dispatch, and persistent conversation history.

### [AI text splitting](./ai-text-splitting-py)

Python examples for length based, recursive, language aware, and semantic text splitting. These scripts make chunk boundaries visible before the same ideas are used in a RAG pipeline.

### [AI PDF RAG app](./ai-pdf-rag-app)

A local question answering application for selected parts of the British Columbia Occupational Health and Safety Regulation. It extracts text with PdfPig, creates overlapping chunks, embeds them with Ollama, stores them in Qdrant, and returns answers with page references.

### [AI PDF RAG API](./ai-pdf-rag-api)

An ASP.NET Core API for the PDF regulation RAG pipeline. It references `ai-pdf-rag-app`, uses the same `AddRagServices()` dependency registration, and exposes the query flow through `POST /api/regulation/query`.

## Shared infrastructure

The .NET applications use the same local Ollama and Qdrant services. Each application uses a separate collection name so their records do not conflict.

Start Qdrant:

```bash
docker run --rm --name ai-rag-qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant
```

Pull the models used across the examples:

```bash
ollama pull gemma3
ollama pull qwen3
ollama pull nomic-embed-text
```

Each project README lists its exact setup, run command, design choices, and current limits.

## Debug locally

The repository includes shared VS Code launch profiles in `.vscode/launch.json`. Choose a `C#` profile to build and debug one of the .NET applications, or choose a `Python` profile to run one text splitting script with breakpoints. Select the Python interpreter from `ai-text-splitting-py/.venv` after installing its requirements.

## Learning guides

[GitHub Actions and open model code review](Docs/github-actions-learning-guide.md) explains why the repository uses CI, Dependabot, and Hugging Face review, how the workflows operate, and how to reproduce the setup in another repository.

## Continuous integration

GitHub Actions builds each .NET project with its target SDK, verifies C# formatting, and compiles the Python scripts for syntax errors. CI does not start Ollama or Qdrant and does not download AI models. [Dependabot](.github/dependabot.yml) checks NuGet, pip, and GitHub Actions dependencies weekly with grouped updates and capped open pull requests.

## Local verification

Run the same checks locally:

```bash
dotnet build ai-rag-movie-app/ai-rag-movie-app.csproj --configuration Release
dotnet build ai-semantic-rag-movie-app/ai-semantic-rag-movie-app.csproj --configuration Release
dotnet build ai-pdf-rag-app/ai-pdf-rag-app.csproj --configuration Release
dotnet build ai-pdf-rag-api/ai-pdf-rag-api.csproj --configuration Release
dotnet test ai-pdf-rag-api.Tests/ai-pdf-rag-api.Tests.csproj --configuration Release

dotnet format ai-rag-movie-app/ai-rag-movie-app.csproj --verify-no-changes
dotnet format ai-semantic-rag-movie-app/ai-semantic-rag-movie-app.csproj --verify-no-changes
dotnet format ai-pdf-rag-app/ai-pdf-rag-app.csproj --verify-no-changes
dotnet format ai-pdf-rag-api/ai-pdf-rag-api.csproj --verify-no-changes
dotnet format ai-pdf-rag-api.Tests/ai-pdf-rag-api.Tests.csproj --verify-no-changes

python3 -m compileall -q ai-text-splitting-py
```

## Automated pull request review

Internal, non draft pull requests targeting `main` receive an advisory review from `Qwen/Qwen2.5-Coder-32B-Instruct` through Hugging Face Inference Providers. The workflow validates structured findings against changed lines, posts valid findings inline, maintains one consolidated summary, and applies one result label. Add `ai:code-review:requested` to run it again manually.

Model review does not approve a pull request or authorize a merge. CI and human judgment remain authoritative. Setup, label meanings, limits, and security controls are covered in the [full guide](Docs/github-actions-learning-guide.md).

## Coding agent configuration

Shared coding agent guidance lives in the root [CLAUDE.md](CLAUDE.md). The repository does not use a pre commit framework or a `.claude` settings folder. Local hooks require separate installation and can be bypassed, while CI runs consistently for every pull request. Tool specific Claude hooks and permissions are unnecessary because pull request review uses Hugging Face.
