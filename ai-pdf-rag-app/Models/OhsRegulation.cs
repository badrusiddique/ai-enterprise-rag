using Microsoft.Extensions.VectorData;

namespace AiPdfRagApp.Models;

public sealed class OhsRegulation
{
    [VectorStoreRecordKey]
    public Guid Id { get; init; } = Guid.NewGuid();

    [VectorStoreRecordData]
    public int PageNumber { get; init; }

    [VectorStoreRecordData]
    public string Content { get; init; } = string.Empty;

    [VectorStoreRecordVector(
        Dimensions: 768,
        DistanceFunction = DistanceFunction.CosineSimilarity,
        IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
