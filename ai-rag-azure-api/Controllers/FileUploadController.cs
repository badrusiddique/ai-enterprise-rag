using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Tags("Controllers/FileUpload")]
public class FileUploadController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public FileUploadController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult<FileUploadResponse>> UploadFile([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var containerClient = new BlobContainerClient(_configuration["AzureBlobStorage:ConnectionString"], _configuration["AzureBlobStorage:ContainerName"]);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(file.FileName);
        await using var fileStream = file.OpenReadStream();
        await blobClient.UploadAsync(fileStream, overwrite: true);

        return Ok(new FileUploadResponse(file.FileName));
    }
}

public record FileUploadResponse(string FileName);
