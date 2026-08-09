using AiRagMovieApp;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

// Grounded strictly on supplied records — the model must not draw on outside knowledge
const string SystemPrompt = """
    You are a film database assistant. You answer questions using only the movie
    records supplied in each request.

    Rules:
    - Use only the supplied records. Do not draw on outside knowledge about films,
      actors, or directors, even if you are confident it is correct.
    - If the records fully answer the question, answer directly and concisely.
    - If they answer only part of it, answer that part and state plainly what the
      records do not cover.
    - If they do not answer it, reply exactly: "I don't have that in the database."
    - Absence is not evidence. A record that omits something tells you nothing
      about it. Never infer, estimate, or fill a gap.
    - Quote titles, names, and years exactly as they appear. Do not correct or
      normalise them.
    """;

Console.WriteLine("Welcome to the AI RAG Movie App!");

var qdrantEndpoint = new Uri("http://localhost:6334");
var ollamaEndpoint = new Uri("http://localhost:11434");

var conversationMemory = new ConversationMemory();
var ollamaChatClient = new OllamaChatClient(ollamaEndpoint, "gemma3:latest");
var embeddingGenerator = new OllamaEmbeddingGenerator(ollamaEndpoint, "nomic-embed-text:latest");

var qdrantClient = new QdrantClient(qdrantEndpoint);
var qdrantVectorStore = new QdrantVectorStore(qdrantClient, true, new QdrantVectorStoreOptions { EmbeddingGenerator = embeddingGenerator });
var movieStore = qdrantVectorStore.GetCollection<ulong, Movie>("00-movies");

var collectionExists = await qdrantClient.CollectionExistsAsync("00-movies");
if (!collectionExists)
{
    Console.WriteLine("Creating collection and populating with movie data...");
    await movieStore.EnsureCollectionExistsAsync();

    foreach (var movie in MovieRepository.GetMovies())
    {
        movie.Embedding = await embeddingGenerator.GenerateVectorAsync(movie.Description);
        await movieStore.UpsertAsync(movie);
    }

    Console.WriteLine("Collection created and populated.");
}

while (true)
{
    Console.Write("What is your question? ");
    var query = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(query))
        continue;

    if (string.Equals(query, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    conversationMemory.AddMessage($"User: {query}");

    // Include prior turns so follow-up questions stay contextually accurate
    var contextQuery = string.Join(" ", conversationMemory.GetConversationHistory());
    var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(contextQuery);

    var references = new HashSet<string>();
    var searchResults = new HashSet<string>();

    var results = movieStore.SearchAsync(queryEmbedding, 5, new VectorSearchOptions<Movie> { VectorProperty = x => x.Embedding });
    await foreach (var result in results)
    {
        if ((result.Score ?? 0) < 0.50) continue;

        searchResults.Add(
            $"Title: {result.Record.Title}, Description: {result.Record.Description}, Reference: {result.Record.Reference}");

        var score = (result.Score ?? 0) * 100;
        references.Add($"{score:F2}% - {result.Record.Reference}");
    }

    var prompt = $"""
        <movies>
        {string.Join(Environment.NewLine, searchResults)}
        </movies>

        Question: {query}
        """;

    var chatResponse = await ollamaChatClient.GetResponseAsync([
        new ChatMessage(ChatRole.System, SystemPrompt),
        new ChatMessage(ChatRole.User, prompt)
    ]);

    conversationMemory.AddMessage($"Assistant: {chatResponse.Text}");
    Console.WriteLine(chatResponse.Text);

    Console.WriteLine("References:");
    foreach (var reference in references)
        Console.WriteLine(reference);
}

Console.WriteLine("Done");
