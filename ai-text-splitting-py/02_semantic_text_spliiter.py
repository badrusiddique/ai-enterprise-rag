from langchain_experimental.text_splitter import SemanticChunker
from langchain_openai import OpenAIEmbeddings
from langchain_huggingface import HuggingFaceEmbeddings

raw_text = """
Farmers were working hard in the fields, preparing the soil and planting seeds for the next season. The sun was bright, and the air smelled of earth and fresh grass. The Indian Premier League (IPL) is the biggest cricket league in the world. People all over the world watch the matches and cheer for their favourite teams.


Terrorism is a big danger to peace and safety. It causes harm to people and creates fear in cities and villages. When such attacks happen, they leave behind pain and sadness. To fight terrorism, we need strong laws, alert security forces, and support from people who care about peace and safety.
"""

# 1. Load your open-source Hugging Face embedding model
# This downloads and runs the model completely locally on your machine
embeddings = HuggingFaceEmbeddings(
    model_name="BAAI/bge-large-en-v1.5",
    model_kwargs={'device': 'cpu'} # Change to 'cuda' if you have an Nvidia GPU
)

semantic_splitter = SemanticChunker(
    embeddings,
    breakpoint_threshold_type="standard_deviation", # options: percentile, standard_deviation, interquartile, gradient
    breakpoint_threshold_amount=0.75        # customizes the threshold behavior
)

semantic_result = semantic_splitter.split_text(raw_text)
print('Semantic text split result:')
print(len(semantic_result))
print(semantic_result)