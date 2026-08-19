using AiPdfRagApp.Configuration;
using AiPdfRagApp.Interfaces;
using AiPdfRagApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

var localSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Local.json");
var localSettingsLoaded = File.Exists(localSettingsPath);
var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? "Production";
var settingsProfile = Environment.GetEnvironmentVariable("APP_SETTINGS_PROFILE")
    ?? (localSettingsLoaded ? "Local" : environmentName);

var ragOptions = new RagOptions();
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.Local.json", optional: true)
    .Build();
configuration.GetSection("Rag").Bind(ragOptions);

var builder = Kernel.CreateBuilder();
builder.Services.AddRagServices(ragOptions);

var kernel = builder.Build();
var ingestionService = kernel.Services.GetRequiredService<PdfIngestionService>();
var queryService = kernel.Services.GetRequiredService<IRegulationQueryService>();

Console.WriteLine("AI PDF RAG App is starting...");
Console.WriteLine($"Environment: {environmentName}");
Console.WriteLine($"Settings profile: {settingsProfile}");
Console.WriteLine($"Local settings: {(localSettingsLoaded ? "loaded" : "not found")}");

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
