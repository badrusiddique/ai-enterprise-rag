using System.Diagnostics;
using AiPdfRagApp.Configuration;
using AiPdfRagApp.Models;
using AiPdfRagApp.Utilities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Text;

namespace AiPdfRagApp.Services;

public sealed class PdfIngestionService(
    IVectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IPdfTextExtractor pdfTextExtractor,
    RagOptions options)
{
    public async Task<(bool Created, int PageCount, int ChunkCount)> EnsureCollectionAsync()
    {
        if (await vectorStore.CollectionExistsAsync(options.CollectionName))
        {
            return (false, 0, 0);
        }

        var pages = pdfTextExtractor.ExtractTextFromPdf(options.PdfPath);
        var regulations = CreateRegulationChunks(pages);
        var collection = vectorStore.GetCollection<Guid, OhsRegulation>(options.CollectionName);

        await collection.CreateCollectionIfNotExistsAsync();

        try
        {
            foreach (var regulation in regulations)
            {
                regulation.Embedding = await embeddingGenerator.GenerateVectorAsync(regulation.Content);
                await collection.UpsertAsync(regulation);
            }
        }
        catch
        {
            // A collection is only ready after every chunk has been stored.
            await vectorStore.DeleteCollectionAsync(options.CollectionName);
            throw;
        }

        return (true, pages.Count, regulations.Count);
    }

#pragma warning disable SKEXP0050
    private List<OhsRegulation> CreateRegulationChunks(
        IList<(int pageNumber, string pageText)> pages)
    {
        var regulations = new List<OhsRegulation>();

        foreach (var (pageNumber, pageText) in pages)
        {
            Debug.WriteLine($"Page {pageNumber}: {pageText}");

            var lines = TextChunker.SplitPlainTextLines(
                pageText,
                options.MaximumTokensPerLine);
            var chunks = TextChunker.SplitPlainTextParagraphs(
                lines,
                options.MaximumTokensPerChunk,
                options.OverlapTokens);

            regulations.AddRange(chunks
                .Where(chunk => !string.IsNullOrWhiteSpace(chunk))
                .Select(chunk => new OhsRegulation
                {
                    PageNumber = pageNumber,
                    Content = chunk
                }));
        }

        return regulations;
    }
#pragma warning restore SKEXP0050
}
