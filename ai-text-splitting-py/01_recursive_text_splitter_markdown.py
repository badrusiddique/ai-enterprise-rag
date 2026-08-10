# Language.MARKDOWN tells the splitter to use Markdown-aware separators:
# headings (##, #), fences (```), blank lines, then words.
# Chunks will not cut through a heading or code fence.
from langchain_text_splitters import Language, RecursiveCharacterTextSplitter


raw_text = """
# Project Name: Smart Student Tracker

A simple Python-based project to manage and track student data, including their grades, age, and academic status.


## Features

- Add new students with relevant info
- View student details
- Check if a student is passing
- Easily extendable class-based design


## 🛠 Tech Stack

- Python 3.10+
- No external dependencies


## Getting Started

1. Clone the repo  
   ```bash
   git clone https://github.com/your-username/student-tracker.git
"""

# from_language is a factory that sets the separator list for the chosen language
markdown_splitter = RecursiveCharacterTextSplitter.from_language(
    language=Language.MARKDOWN,
    chunk_size=100,
    chunk_overlap=0,
)

raw_result = markdown_splitter.split_text(raw_text)
print('Raw text split result:')
print(len(raw_result))  # each heading section becomes its own chunk
print(raw_result)
