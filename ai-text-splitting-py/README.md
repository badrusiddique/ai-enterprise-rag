# AI Text Splitting

A collection of Python scripts exploring the three main text splitting strategies used in RAG pipelines. Each script is self-contained and runnable independently.

## Why text splitting matters

Before you can search a document semantically, you need to break it into chunks. The chunk strategy directly affects retrieval quality:

- Chunks too large: irrelevant content gets injected into the prompt
- Chunks too small: useful context gets cut off mid-thought
- Wrong boundary strategy: sentences or code blocks get split in the middle

These scripts demonstrate each approach with concrete examples so the tradeoffs are visible.

## Strategies

### 00 - Length-based splitting (CharacterTextSplitter)

Splits on a fixed character count using a configurable separator.

```
raw text
    |
    v
split every N characters at separator boundary
    |
    v
[chunk1, chunk2, chunk3, ...]
```

Best for: uniform, unstructured prose where sentence boundaries do not matter much.

### 01 - Recursive splitting (RecursiveCharacterTextSplitter)

Tries a list of separators in priority order (paragraph, sentence, word, character) until chunks fit within the size limit. Language-aware variants understand Markdown headings, Python class/function boundaries, and more.

```
raw text
    |
    v
try split on "\n\n" -> chunks too big?
    |
    v
try split on "\n" -> chunks too big?
    |
    v
try split on " " -> done
    |
    v
[chunk1, chunk2, chunk3, ...]
```

Variants in this project:
- `01_recursive_text_splitter.py` - plain text
- `01_recursive_text_splitter_markdown.py` - respects Markdown headings and sections
- `01_recursive_text_splitter_python.py` - respects Python class and function boundaries

Best for: most RAG use cases. The language-aware variant is the right default for code.

### 02 - Semantic splitting (SemanticChunker)

Embeds each sentence, then finds breakpoints where the semantic similarity between adjacent sentences drops below a threshold. Chunks end where the topic changes, not where the character count hits a limit.

```
raw text -> sentences
    |
    v
embed each sentence (HuggingFace BAAI/bge-large-en-v1.5, local)
    |
    v
measure cosine distance between adjacent sentence embeddings
    |
    v
insert breakpoint where distance exceeds threshold
    |
    v
[topically coherent chunk1, chunk2, ...]
```

Best for: mixed-topic documents where you want each chunk to cover exactly one idea. Slower than length-based — embeddings run at split time, not just at query time.

## Comparison

| Strategy | Boundary type | Speed | Coherence | Best for |
|----------|--------------|-------|-----------|----------|
| Character | Fixed count | Fast | Low | Uniform prose |
| Recursive | Linguistic | Fast | Medium | General purpose |
| Semantic | Topic shift | Slow | High | Mixed-topic docs |

## Setup

```bash
cd ai-text-splitting-py
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

## Running a script

```bash
python 00_text_length_based_splitter.py
python 01_recursive_text_splitter.py
python 01_recursive_text_splitter_markdown.py
python 01_recursive_text_splitter_python.py
python 02_semantic_text_spliiter.py
```

The semantic splitter downloads the `BAAI/bge-large-en-v1.5` model on first run (~1.3 GB). Subsequent runs use the cached model.

## Dataset

`00_corpus_doc.pdf` is a sample document used by the length-based splitter to demonstrate splitting loaded PDF pages.
