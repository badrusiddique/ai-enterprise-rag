# RecursiveCharacterTextSplitter tries a list of separators in order:
# ["\n\n", "\n", " ", ""] — paragraph, line, word, then individual characters.
# It works down the list until chunks fit within chunk_size.
# This preserves sentence and paragraph boundaries where possible.
from langchain_text_splitters import RecursiveCharacterTextSplitter

raw_text = """
Space exploration has led to incredible scientific discoveries. From landing on the Moon to exploring Mars, humanity continues to push the boundaries of what’s possible beyond our planet.

These missions have not only expanded our knowledge of the universe but have also contributed to advancements in technology here on Earth. Satellite communications, GPS, and even certain medical imaging techniques trace their roots back to innovations driven by space programs.
"""

recursive_splitter = RecursiveCharacterTextSplitter(
    chunk_size=100,  # maximum characters per chunk
    chunk_overlap=0, # characters shared between adjacent chunks (0 = no overlap)
)

raw_result = recursive_splitter.split_text(raw_text)
print('Raw text split result:')
print(len(raw_result))  # number of chunks produced
print(raw_result)
