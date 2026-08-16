# AI RAG Movie App

A console application that demonstrates Retrieval-Augmented Generation (RAG) using a local movie database. Built to explore how to ground LLM responses in a trusted dataset rather than letting the model rely on its own training data.

## Why RAG?

Large language models hallucinate. When asked about specific facts like film details, cast, or plot, they often produce plausible-sounding but incorrect answers with full confidence.

RAG fixes this by shifting the burden of knowledge away from the model:

1. **Store your data as vectors** in a dedicated vector database (Qdrant)
2. **Search semantically** for the most relevant records at query time
3. **Inject only those records** into the prompt. The model sees nothing else.

The LLM is explicitly instructed to answer only from what is supplied. If the data does not cover the question, it says so. Responses are reliable, auditable, and traceable to a source.

## How it works

```
User query
    |
    v
Embed query with nomic-embed-text (Ollama)
    |
    v
Semantic search in Qdrant (top 5, cosine similarity, score >= 50%)
    |
    v
Inject matching movie records into prompt
    |
    v
LLM response via gemma3 (Ollama), grounded with Wikipedia references
```

Conversation history is accumulated across turns so follow-up questions remain contextually accurate.

## Stack

| Component                | Tool                                          |
|--------------------------|-----------------------------------------------|
| Vector database          | [Qdrant](https://qdrant.tech), local via Docker |
| Embeddings               | `nomic-embed-text` via Ollama, 768-dim, HNSW  |
| Chat model               | `gemma3` via Ollama, fully local, no API key  |
| Vector store abstraction | `Microsoft.Extensions.AI` + Semantic Kernel   |

## Prerequisites

- [Docker](https://www.docker.com/)
- [Ollama](https://ollama.com/)
- .NET 10 SDK

## Running locally

**1. Start Qdrant**

```bash
docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant
```

**2. Pull the models**

```bash
ollama pull gemma3
ollama pull nomic-embed-text
```

**3. Run the app**

```bash
cd ai-rag-movie-app
dotnet run
```

On first run the app creates the `00-movies` collection in Qdrant and generates embeddings for all 20 films. Subsequent runs skip this and query the existing collection directly.

Type `exit` to quit.

To debug this project in VS Code, choose `C#: AI RAG movie app` from the Run and Debug menu. The profile builds the Debug target before starting the app.

## Dataset

20 curated films grouped by director:

| Director                | Films                                         |
|-------------------------|-----------------------------------------------|
| Christopher Nolan       | The Dark Knight Trilogy                       |
| Denis Villeneuve        | Dune: Part One, Dune: Part Two                |
| Quentin Tarantino       | Pulp Fiction, Reservoir Dogs                  |
| Martin Scorsese         | Goodfellas, Casino                            |
| Francis Ford Coppola    | The Godfather, The Godfather Part II          |
| Ridley Scott            | Gladiator, Kingdom of Heaven                  |
| Sony Pictures Animation | Spider-Man: Into/Across the Spider-Verse      |
| James Cameron           | The Terminator, Terminator 2: Judgment Day    |
| David Fincher           | Se7en, Fight Club, Zodiac                     |

Each record stores a title, description, and Wikipedia reference URL. The embedding is generated from the description field at startup.
