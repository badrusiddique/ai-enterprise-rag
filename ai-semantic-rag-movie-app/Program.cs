#pragma warning disable SKEXP0070

using AiSemanticRagMovieApp;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Qdrant.Client;

// Tools are invoked automatically — the model decides when to call CompareMovies or Now
const string SystemPrompt = """
    You are a film database assistant.

    You have access to tools — use them when they are the right way to answer.
    Use the CompareMovies tool when the user asks to compare two movies.
    Use the Now tool when the user asks for the current date or time.

    For all other questions, use only the movie records supplied in each request.
    If the records do not answer the question, say "I don't have that in the database."
    """;

Console.WriteLine("Welcome to the AI Semantic RAG Movie App");

var qdrantEndpoint = new Uri("http://localhost:6334");
var ollamaEndpoint = new Uri("http://localhost:11434");

var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOllamaChatCompletion(modelId: "qwen3:latest", endpoint: ollamaEndpoint);
kernelBuilder.AddOllamaEmbeddingGenerator(modelId: "nomic-embed-text:latest", endpoint: ollamaEndpoint);
kernelBuilder.Services.AddQdrantVectorStore();
kernelBuilder.Plugins.AddFromType<MoviePlugin>();
kernelBuilder.Services.AddSingleton(_ => new QdrantClient(qdrantEndpoint));

var kernel = kernelBuilder.Build();

var qdrantVectorStore = kernel.Services.GetRequiredService<IVectorStore>();
var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
var embeddingGenerator = kernel.Services.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
var moviesStore = qdrantVectorStore.GetCollection<ulong, Movie>("01-movies");

var collectionExists = await qdrantVectorStore.CollectionExistsAsync("01-movies");
if (!collectionExists)
{
    Console.WriteLine("Creating collection and adding movies to the vector store...");
    await moviesStore.CreateCollectionIfNotExistsAsync();

    foreach (var movie in MovieRepository.GetMovies())
    {
        movie.Embedding = await embeddingGenerator.GenerateVectorAsync(movie.Description);
        await moviesStore.UpsertAsync(movie);
    }

    Console.WriteLine("Movies added to the vector store.");
}

// Carry last retrieved titles into next embedding to anchor follow-up questions
var lastRetrievedTitles = "";
var chatHistory = new ChatHistory(SystemPrompt);

while (true)
{
    Console.Write("What is your question? ");
    var query = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(query))
        continue;

    if (string.Equals(query, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    var searchResults = new List<string>();
    var searchReferences = new List<string>();

    var queryEmbedding = await embeddingGenerator.GenerateVectorAsync($"{lastRetrievedTitles} {query}");
    var results = moviesStore.SearchEmbeddingAsync(queryEmbedding, 3, new VectorSearchOptions<Movie>
    {
        VectorProperty = x => x.Embedding
    });

    await foreach (var result in results)
    {
        var movieResult = result.Record;
        searchResults.Add($"Title: {movieResult.Title}, Year: {movieResult.Year}, Description: {movieResult.Description}, Reference: {movieResult.Reference}");

        var resultScore = (result.Score ?? 0) * 100;
        searchReferences.Add($"{resultScore:F2}% - {movieResult.Reference}");
    }

    lastRetrievedTitles = searchResults.FirstOrDefault() ?? "";

    var userPrompt = $"""
        Current Context:
        {string.Join("\n", searchResults)}

        Query: {query}
        """;

    chatHistory.AddUserMessage(userPrompt);

    var chatResult = await chatCompletionService.GetChatMessageContentAsync(chatHistory, new PromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        ExtensionData = new Dictionary<string, object> { { "think", false } }
    }, kernel);

    chatHistory.AddAssistantMessage(chatResult.Content ?? string.Empty);
    Console.WriteLine(chatResult.Content);

    Console.WriteLine("References:");
    foreach (var searchReference in searchReferences)
        Console.WriteLine(searchReference);
}

Console.WriteLine("Done");

#pragma warning restore SKEXP0070
