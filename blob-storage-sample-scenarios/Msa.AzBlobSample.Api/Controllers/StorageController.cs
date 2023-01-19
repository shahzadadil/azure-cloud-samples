using System.Collections.Immutable;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

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
		var connetionString = this._Configuration.GetValue<String>("Storage:ConnectionString");
        BlobServiceClient storageClient = new(connetionString);
		BlobContainerClient container = storageClient.GetBlobContainerClient(_ImageContainerName);
		await container.CreateIfNotExistsAsync();
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


}
