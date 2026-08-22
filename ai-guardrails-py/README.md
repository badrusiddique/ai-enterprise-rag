# AI Guardrails Python

This notebook project explores input validation with [Guardrails AI](https://www.guardrailsai.com/). Each notebook uses a focused validator and shows how different `on_fail` policies affect the validated output.

## Notebooks

1. `notebooks/ai-competitor-check.ipynb` rejects configured competitor names while allowing ordinary uses of ambiguous words such as "apple".
2. `notebooks/ai-jailbreak-check.ipynb` detects jailbreak-style prompts and demonstrates `exception` and `refrain` policies.
3. `notebooks/ai-pii-check.ipynb` detects selected PII entity types and demonstrates `exception` and `refrain` policies.

The validators use spaCy's `en_core_web_trf` language model. The notebooks include setup cells for the extra spaCy transformer packages and model download.

## Setup

From this directory, create and activate a virtual environment:

```bash
python3 -m venv .venv
source .venv/bin/activate
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
python -m ipykernel install --user --name ai-guardrails-py --display-name "AI Guardrails Python"
```

The project `.gitignore` excludes `.venv` and `.env`. Create `.env` locally only when a notebook or provider configuration needs environment variables; do not commit credentials.

## Run

Open either the repository or this folder in VS Code, select the `AI Guardrails Python` kernel, and run a notebook from `notebooks/` cell by cell. Start with the setup cells when the spaCy transformer model has not been installed in the active environment.

## Scope

These examples demonstrate validator behavior, not a complete application policy. Treat the configured competitors and PII entities as sample inputs that should be tailored to the product and data being protected.
