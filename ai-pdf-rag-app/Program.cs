using AiPdfRagApp.Configuration;
using AiPdfRagApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();
builder.Services.AddRagServices();

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
