
using Microsoft.SemanticKernel;

public interface IAzureOpenAiService
{
    Task<string> AskAnythingAsync(IList<string> context, string query);
}

public class AzureOpenAiService : IAzureOpenAiService
{
    private readonly Kernel _kernel;

    public AzureOpenAiService(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> AskAnythingAsync(IList<string> context, string query)
    {
        if (context == null || context.Count == 0 || string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Context and query cannot be null or empty.");
        }

        var prompt = """
            You are a BC Occupational Health and Safety regulation assistant.
            Answer questions using ONLY the regulation text provided in the context below.
            Always cite the page number your answer comes from.
            If the answer cannot be found in the provided context, respond with exactly:
            "I don't have that information in the regulation text provided."
            Do not use any prior knowledge outside of the provided context.

            Context:
            {{$context}}

            Question:
            {{$query}}
            """;

        var result = await _kernel.InvokePromptAsync(prompt,new KernelArguments { ["context"] = string.Join("\n", context), ["query"] = query });

        return result.ToString() ?? string.Empty;
    }
}
