using Microsoft.Extensions.VectorData;

namespace AiRagMovieApp;

public class Movie
{
    [VectorStoreKey]
    public ulong Id { get; set; }

    [VectorStoreData]
    public required string Title { get; set; }

    [VectorStoreData]
    public required string Description { get; set; }

    [VectorStoreData]
    public required string Reference { get; set; }

    [VectorStoreVector(Dimensions: 768, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
