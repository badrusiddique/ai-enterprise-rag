# Hugging Face review test

This temporary file verifies the pull request automation without changing application behavior.

Expected checks:

1. The CI workflow builds and formats each .NET project.
2. The CI workflow compiles the Python scripts.
3. The Hugging Face workflow posts one advisory review comment.
4. A later push updates the existing review comment instead of creating a duplicate.

This pull request should be closed after the automation is verified.
