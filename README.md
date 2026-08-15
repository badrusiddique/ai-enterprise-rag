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

## Repository checks

GitHub Actions builds each .NET project with its target SDK, verifies C# formatting, and compiles the Python scripts for syntax errors. CI does not start Ollama or Qdrant and does not download AI models.

Run the same checks locally:

```bash
dotnet build ai-rag-movie-app/ai-rag-movie-app.csproj --configuration Release
dotnet build ai-semantic-rag-movie-app/ai-semantic-rag-movie-app.csproj --configuration Release
dotnet build ai-pdf-rag-app/ai-pdf-rag-app.csproj --configuration Release

dotnet format ai-rag-movie-app/ai-rag-movie-app.csproj --verify-no-changes
dotnet format ai-semantic-rag-movie-app/ai-semantic-rag-movie-app.csproj --verify-no-changes
dotnet format ai-pdf-rag-app/ai-pdf-rag-app.csproj --verify-no-changes

python3 -m compileall -q ai-text-splitting-py
```

Dependabot checks NuGet, pip, and GitHub Actions dependencies weekly. Related updates are grouped and the number of open update pull requests is limited.

## Automated pull request review

Internal, non draft pull requests are reviewed by the open source `Qwen/Qwen2.5-Coder-32B-Instruct` model through Hugging Face Inference Providers. The workflow reads the pull request diff through the GitHub API, limits the submitted diff to 60 KB, and updates one advisory review comment. It does not check out or execute pull request code.

Configure the reviewer:

1. Create a fine grained Hugging Face token with Inference Providers permission.
2. Add it as an Actions repository secret named `HF_TOKEN`.
3. Optionally add an Actions repository variable named `HF_REVIEW_MODEL` to select another chat completion model.

Repository secrets and variables are configured under `Settings`, `Secrets and variables`, then `Actions`.

The review workflow skips forked pull requests, draft pull requests, and Dependabot pull requests. Model output is advisory. The deterministic CI workflow remains the quality gate and should be the required status check in the `main` branch ruleset.

Shared coding agent guidance lives in the root [CLAUDE.md](CLAUDE.md). The repository does not use a pre commit framework or a `.claude` settings folder. Local hooks require separate installation and can be bypassed, while CI runs consistently for every pull request. Tool specific Claude hooks and permissions are unnecessary because pull request review uses Hugging Face.
