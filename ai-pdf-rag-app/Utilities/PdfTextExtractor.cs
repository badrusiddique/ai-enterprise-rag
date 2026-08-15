using UglyToad.PdfPig;

namespace AiPdfRagApp.Utilities;

public interface IPdfTextExtractor
{
    IList<(int pageNumber, string pageText)> ExtractTextFromPdf(string filePath);
}

public sealed class PdfTextExtractor : IPdfTextExtractor
{
    public IList<(int pageNumber, string pageText)> ExtractTextFromPdf(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The regulation PDF could not be found.", filePath);
        }

        using var document = PdfDocument.Open(filePath);

        return document.GetPages()
            .Select(page => (page.Number, page.Text))
            .ToList();
    }
}
