using Microsoft.Extensions.VectorData;

namespace AiSemanticRagMovieApp;

public class Movie
{
    [VectorStoreRecordKey]
    public ulong Id { get; set; }

    [VectorStoreRecordData]
    public required string Title { get; set; }

    [VectorStoreRecordData]
    public int Year { get; set; }

    [VectorStoreRecordData]
    public required string Description { get; set; }

    [VectorStoreRecordData]
    public required string Reference { get; set; }

    [VectorStoreRecordVector(Dimensions: 768, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
