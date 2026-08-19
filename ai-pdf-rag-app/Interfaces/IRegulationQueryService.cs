namespace AiPdfRagApp.Interfaces;

public interface IRegulationQueryService
{
    Task<(string Answer, IReadOnlyList<string> References)> AskAsync(string question);
}
