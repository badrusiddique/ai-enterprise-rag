# AI Semantic RAG Movie App

A console application that extends the RAG pattern with **Semantic Kernel**, adding function calling, tool dispatch, and persistent chat history to a grounded movie assistant.

## Why this version?

The [basic RAG app](../ai-rag-movie-app/README.md) grounds the LLM in a vector database. This version adds two things:

1. **Function calling**: the model can invoke registered tools at its own discretion. Comparison logic and time-sensitive queries are routed to C# methods automatically, not hardcoded in the prompt.

2. **Persistent chat history**: instead of a rolling window, the full `ChatHistory` object is passed to the model each turn. This gives the model the complete conversation context, not just a string concatenation.

These are the patterns you would use in a real agent, not just a Q&A bot.

## How it works

```
User query
    |
    v
Embed (lastRetrievedTitles + query) with nomic-embed-text
    |
    v
Semantic search in Qdrant (top 3, cosine similarity)
    |
    v
Inject matching records into prompt
    |
    v
SK ChatHistory -> qwen3 (Ollama) with FunctionChoiceBehavior.Auto
    |
    |-- answer from context -> response
    |-- tool call detected -> invoke plugin -> response
```

The embedding includes the title of the last retrieved result to bias follow-up searches toward the same film cluster.

## Tools (MoviePlugin)

| Tool | Trigger | What it does |
|------|---------|--------------|
| `Now` | "what time is it", "today's date" | Returns `DateTime.Now` |
| `CompareMovies` | "compare X and Y" | Looks up both films and returns a side-by-side summary |

The model decides when to call these. No keyword matching needed.

## Stack

| Component              | Tool                                                       |
|------------------------|------------------------------------------------------------|
| Vector database        | [Qdrant](https://qdrant.tech), local via Docker            |
| Embeddings             | `nomic-embed-text` via `SemanticKernel.Connectors.Ollama`  |
| Chat model             | `qwen3` via Ollama, supports thinking and tool use         |
| Orchestration          | Microsoft Semantic Kernel (DI, plugins, chat history)      |

## Why qwen3 instead of gemma3?

`qwen3` reliably follows tool-calling protocols and supports the `think: false` extension to suppress chain-of-thought output. `gemma3` works well for straight Q&A but is less consistent with structured function dispatch.

## Prerequisites

- [Docker](https://www.docker.com/)
- [Ollama](https://ollama.com/)
- .NET 9 SDK

## Running locally

**1. Start Qdrant**

```bash
docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant
```

**2. Pull the models**

```bash
ollama pull qwen3
ollama pull nomic-embed-text
```

**3. Run the app**

```bash
cd ai-semantic-rag-movie-app
dotnet run
```

On first run the app creates the `01-movies` collection in Qdrant and generates embeddings. Subsequent runs reuse the existing collection.

Type `exit` to quit.

To debug this project in VS Code, choose `C#: Semantic RAG movie app` from the Run and Debug menu. The profile builds the Debug target before starting the app.

## Dataset

Same 20 films as the base app, grouped by director. Descriptions in this version include director, year, and lead cast for richer context when the LLM reasons over records.

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

