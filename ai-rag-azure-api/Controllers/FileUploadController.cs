using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public sealed class AzureBlobStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ConnectionString)
        && !string.IsNullOrWhiteSpace(ContainerName);
}

[ApiController]
[Route("api/[controller]")]
[Tags("Controllers/FileUpload")]
public class FileUploadController : ControllerBase
{
    private readonly AzureBlobStorageOptions _options;

    public FileUploadController(IOptions<AzureBlobStorageOptions> options)
    {
        _options = options.Value;
    }

    [HttpPost]
    public async Task<ActionResult<FileUploadResponse>> UploadFile([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        if (!_options.IsConfigured)
        {
            return StatusCode(500, "Blob Storage configuration is missing.");
        }

        var containerClient = new BlobContainerClient(_options.ConnectionString, _options.ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(file.FileName);
        await using var fileStream = file.OpenReadStream();
        await blobClient.UploadAsync(fileStream, overwrite: true);

        return Ok(new FileUploadResponse(file.FileName));
    }
}

public record FileUploadResponse(string FileName);
