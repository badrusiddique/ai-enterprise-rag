# SemanticChunker splits by topic shift rather than character count.
# It embeds each sentence and inserts a breakpoint where adjacent sentence
# embeddings are far apart in cosine distance.
# Note: langchain-experimental is in maintenance mode but SemanticChunker has no stable replacement yet.
from langchain_experimental.text_splitter import SemanticChunker
from langchain_huggingface import HuggingFaceEmbeddings

raw_text = """
Farmers were working hard in the fields, preparing the soil and planting seeds for the next season. The sun was bright, and the air smelled of earth and fresh grass. The Indian Premier League (IPL) is the biggest cricket league in the world. People all over the world watch the matches and cheer for their favourite teams.


Terrorism is a big danger to peace and safety. It causes harm to people and creates fear in cities and villages. When such attacks happen, they leave behind pain and sadness. To fight terrorism, we need strong laws, alert security forces, and support from people who care about peace and safety.
"""

# BAAI/bge-large-en-v1.5 is a strong open-source embedding model (~1.3 GB).
# It downloads once and is cached locally by HuggingFace.
# Change device to 'cuda' if you have an Nvidia GPU for faster embedding.
embeddings = HuggingFaceEmbeddings(
    model_name="BAAI/bge-large-en-v1.5",
    model_kwargs={'device': 'cpu'}
)

# breakpoint_threshold_type controls how the distance threshold is calculated:
#   percentile          - split where distance is above the Nth percentile
#   standard_deviation  - split where distance is above mean + N * std dev
#   interquartile       - split based on IQR of distances
#   gradient            - split where the rate of change in distance is high
semantic_splitter = SemanticChunker(
    embeddings,
    breakpoint_threshold_type="standard_deviation",
    breakpoint_threshold_amount=0.75  # higher = fewer, larger chunks
)

semantic_result = semantic_splitter.split_text(raw_text)
print('Semantic text split result:')
print(len(semantic_result))
print(semantic_result)