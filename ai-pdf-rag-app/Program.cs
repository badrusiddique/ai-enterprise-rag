using AiPdfRagApp.Configuration;
using AiPdfRagApp.Services;
using AiPdfRagApp.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Qdrant.Client;

var options = new RagOptions();
var builder = Kernel.CreateBuilder();

builder.Services.AddSingleton(options);
builder.Services.AddSingleton<IPdfTextExtractor, PdfTextExtractor>();
builder.Services.AddSingleton<PdfIngestionService>();
builder.Services.AddSingleton<RegulationQueryService>();
builder.Services.AddSingleton(_ => new QdrantClient(options.QdrantEndpoint));
builder.Services.AddQdrantVectorStore();

#pragma warning disable SKEXP0070
builder.AddOllamaEmbeddingGenerator(options.EmbeddingModel, options.OllamaEndpoint);
builder.AddOllamaChatCompletion(options.ChatModel, options.OllamaEndpoint);
#pragma warning restore SKEXP0070

var kernel = builder.Build();
var ingestionService = kernel.Services.GetRequiredService<PdfIngestionService>();
var queryService = kernel.Services.GetRequiredService<RegulationQueryService>();

Console.WriteLine("AI PDF RAG App is starting...");

var ingestion = await ingestionService.EnsureCollectionAsync();
Console.WriteLine(ingestion.Created
    ? $"Indexed {ingestion.ChunkCount} chunks from {ingestion.PageCount} pages."
    : "The Qdrant collection is ready.");

while (true)
{
    Console.WriteLine("---------------------------------------------");
    Console.Write("Enter your query, or type exit to quit: ");
    var question = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(question)
        || question.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var result = await queryService.AskAsync(question);

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine($"Answer: {result.Answer}");
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("Answer references:");

    foreach (var reference in result.References)
    {
        Console.WriteLine(reference);
    }
}

Console.WriteLine("---------------------------------------------");
Console.WriteLine("Done.");
