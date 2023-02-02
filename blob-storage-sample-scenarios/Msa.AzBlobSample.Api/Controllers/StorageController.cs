using System.Collections.Immutable;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.AspNetCore.Mvc;

using Msa.AzBlobSample.Api.Client;

namespace Msa.AzBlobSample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StorageController : ControllerBase
{
    private readonly ILogger<StorageController> _Logger;
    private readonly IConfiguration _Configuration;
    private const String _ImageContainerName = "images";

    public StorageController(ILogger<StorageController> logger, IConfiguration configuration)
    {
        _Logger = logger;
        _Configuration = configuration;
    }

    public async Task<ImmutableDictionary<String, String>> GetAsync()
    {
        try
        {
            var connetionString = _Configuration.GetValue<String>("Storage:ConnectionString");
            BlobServiceClient storageClient = new(connetionString);
            BlobContainerClient container = storageClient.GetBlobContainerClient(_ImageContainerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var containerUri = container.Uri.AbsoluteUri;

            var blobItems = container.GetBlobs();

            if (!blobItems.Any())
            {
                return ImmutableDictionary<String, String>.Empty;
            }

            return blobItems.ToImmutableDictionary(
                item => item.Name,
                item => $"{containerUri}/{item.Name}"
            );
        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpPost]
    public async Task<FileMeta> PostAsync([FromForm] IEnumerable<IFormFile> files)
    {
        try
        {
            var connetionString = _Configuration.GetValue<String>("Storage:ConnectionString");
            BlobServiceClient storageClient = new(connetionString);
            BlobContainerClient container = storageClient.GetBlobContainerClient(_ImageContainerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var containerUri = container.Uri.AbsoluteUri;

            var file = files.FirstOrDefault();

            if (file is null)
            {
                throw new InvalidOperationException();
            }

            var blobClient = container.GetBlobClient(file.FileName);

            if (await blobClient.ExistsAsync())
            {
                throw new InvalidOperationException();
            }

            var fileStream = file.OpenReadStream();

            var blobInfo = await blobClient.UploadAsync(
                fileStream, 
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders() { ContentType = "image/jpeg" }
                });            

            return new FileMeta
            {
                Filename = file.FileName,
                Url = $"{containerUri}/{file.FileName}"
            };

        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpDelete]
    [ActionName("Delete")]
    public async Task DeleteAllAsync()
    {
        try
        {
            var connetionString = _Configuration.GetValue<String>("Storage:ConnectionString");
            BlobServiceClient storageClient = new(connetionString);
            BlobContainerClient container = storageClient.GetBlobContainerClient(_ImageContainerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var blobItems = container.GetBlobs();

            if (!blobItems.Any())
            {
                return;
            }

            foreach (var blob in blobItems)
            {
                await container.DeleteBlobAsync(blob.Name);
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpDelete]
    [Route("Delete/{blobName}")]
    public async Task DeleteAsync([FromRoute]string blobName)
    {
        try
        {
            var connetionString = _Configuration.GetValue<String>("Storage:ConnectionString");
            BlobServiceClient storageClient = new(connetionString);
            BlobContainerClient container = storageClient.GetBlobContainerClient(_ImageContainerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var blobItem = container.GetBlobClient(blobName);

            if (blobItem is null)
            {
                return;
            }

            await blobItem.DeleteAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }
}
