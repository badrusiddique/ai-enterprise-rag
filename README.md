# AI Enterprise RAG

A monorepo of .NET console applications exploring Retrieval-Augmented Generation (RAG) patterns using local models and vector search. Each project builds on the previous, adding more capability while keeping everything running locally with no external API calls.

## Projects

| Project | What it adds |
|---------|--------------|
| [ai-rag-movie-app](./ai-rag-movie-app) | Base RAG pipeline using `Microsoft.Extensions.AI`. Embeds a curated movie dataset into Qdrant and answers questions grounded strictly in that data. |
| [ai-semantic-rag-movie-app](./ai-semantic-rag-movie-app) | Extends the base with Semantic Kernel, adding function calling, tool dispatch via plugins, and full persistent chat history. |
| [ai-text-splitting-py](./ai-text-splitting-py) | Python scripts covering the three main chunking strategies: length-based, recursive (with language-aware variants for Markdown and Python), and semantic splitting using local HuggingFace embeddings. |

## Shared infrastructure

Both apps target the same local Qdrant instance and Ollama server. Collections are prefixed (`00-movies`, `01-movies`) so they do not conflict when running side by side.

**Start Qdrant:**

```bash
docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant
```

**Pull required models:**

```bash
ollama pull gemma3
ollama pull qwen3
ollama pull nomic-embed-text
```

See each project README for app-specific prerequisites and usage.
