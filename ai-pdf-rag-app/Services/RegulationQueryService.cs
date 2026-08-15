using AiPdfRagApp.Configuration;
using AiPdfRagApp.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AiPdfRagApp.Services;

public sealed class RegulationQueryService(
    IVectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IChatCompletionService chatCompletionService,
    RagOptions options)
{
    private const string SystemPrompt = """
        You are a BC Occupational Health and Safety regulation assistant.
        Answer questions using only the regulation text supplied in each request.
        Always cite the page number your answer comes from.
        If the supplied text does not answer the question, say exactly:
        'I don't have that in the regulation text provided.'
        """;

    private readonly ChatHistory chatHistory = new(SystemPrompt);

    public async Task<(string Answer, IReadOnlyList<string> References)> AskAsync(string question)
    {
        var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(question);
        var collection = vectorStore.GetCollection<Guid, OhsRegulation>(options.CollectionName);
        var searchResults = collection.SearchEmbeddingAsync(
            queryEmbedding,
            options.SearchResultCount,
            new VectorSearchOptions<OhsRegulation>
            {
                VectorProperty = regulation => regulation.Embedding
            });

        var context = new List<string>();
        var references = new List<string>();

        await foreach (var result in searchResults)
        {
            var score = result.Score ?? 0;
            context.Add($"Page: {result.Record.PageNumber} Content: {result.Record.Content}");
            references.Add(
                $"Score: {score:P2} Page: {result.Record.PageNumber} Content: {result.Record.Content}");
        }

        var userPrompt = $"""
            Context:
            {string.Join(Environment.NewLine, context)}

            Question: {question}
            """;

        chatHistory.AddUserMessage(userPrompt);
        var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
        var answer = response.Content ?? string.Empty;
        chatHistory.AddAssistantMessage(answer);

        return (answer, references);
    }
}
