# Decision: Week 01 chunking and schema rigidity

Date: 2026-08-15

## Status

Accepted

## Context

The first PDF RAG exercise needs to make document ingestion visible. The source is a regulation PDF with meaningful page references. Answers must remain grounded in retrieved text and cite the page that supplied the context.

The current local stack uses PdfPig, Semantic Kernel text chunking, Ollama, and Qdrant. The `nomic-embed-text` model produces vectors with 768 values. Qdrant collections require a known vector dimension and distance function when the collection is created.

Several parts of this pipeline are rigid by design. Chunk size affects retrieval quality. The embedding model fixes the vector dimension. The vector store record supports one key. An existing Qdrant collection does not automatically change when the source PDF, chunking settings, or embedding model changes.

## Decision

Each PDF page is kept as the unit of provenance. Page text is first split into lines with a limit of 100 tokens. Those lines are then combined into chunks with a limit of 512 tokens and an overlap of 50 tokens.

Each chunk becomes one `OhsRegulation` record with a generated `Guid`, its source page number, its text, and one embedding. The `Guid` is the only vector store key. Page number and content are payload data.

Embeddings use `nomic-embed-text` with 768 dimensions. Qdrant uses cosine similarity and an HNSW index. A query retrieves the three closest records. Their text and page numbers become the context sent to the chat model.

The application creates the collection only when it does not exist. If ingestion fails before every chunk is stored, the application removes the incomplete collection. A schema, source, model, or chunking change still requires an explicit collection rebuild. This behavior is kept visible instead of adding migration and versioning machinery during the first week.

`Program.cs` stays focused on dependency registration and the console flow. PDF ingestion and regulation querying are separate concrete services because they are the two substantial operations in the application. The vector record, shared options, and PDF utility remain separate because they represent concrete data, configuration, and document access. Repository layers, service interfaces, and extra result types are deferred until the application has more than one caller or implementation.

## Why this choice

The 512 token limit gives the model enough local context for a regulation clause without embedding an entire page as one broad record. The 50 token overlap reduces the chance that a sentence or requirement split near a boundary loses its surrounding meaning.

Page level provenance is simple and useful. It allows every retrieved chunk to retain a citation without introducing a separate document metadata system.

Cosine similarity matches the current text embedding use case. HNSW is the connector default chosen for efficient approximate search as the corpus grows.

## Consequences

1. The ingestion path is easy to inspect and explain.
2. Retrieved records can always be traced back to a PDF page.
3. Overlap duplicates some text and increases embedding and storage work.
4. Fixed token values may split legal sections at poor boundaries.
5. A different embedding dimension requires a model change and a collection rebuild.
6. A changed PDF or chunking strategy is not detected automatically.
7. PdfPig reading order can affect chunk quality before token splitting begins.

## Alternatives considered

### One record per page

This would preserve citations with less code, but long pages can mix unrelated requirements and weaken retrieval precision.

### Section aware chunking

Splitting on regulation headings and clause numbers would better match the legal structure. It was deferred because reliable section parsing needs rules that are specific to the source document and would distract from the first complete RAG path.

### Semantic chunking

Semantic boundaries can produce coherent chunks, but they add another model and more ingestion cost. The repository already contains a separate semantic splitting exercise, so this project keeps deterministic chunking for comparison.

### Automatic collection versioning

A fingerprint of the PDF, model, and chunk settings could create a new collection when inputs change. That is useful later, but it adds lifecycle and cleanup work that is not needed for the current corpus.

## Review point

Revisit this decision after retrieval evaluation shows where answers fail. Useful evidence includes missed clauses, weak similarity scores, citations that span the wrong page, and repeated context caused by overlap. Changes should be driven by those results rather than by a larger abstraction alone.