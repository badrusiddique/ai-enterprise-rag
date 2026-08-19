namespace AiPdfRagApp.Services;

public interface IRegulationQueryService
{
    Task<(string Answer, IReadOnlyList<string> References)> AskAsync(string question);
}
