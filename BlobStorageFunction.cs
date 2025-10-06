using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text;

public class BlobStorageFunction
{
    [Function("UploadBlob")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var containerName = "uploads";

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient("example.txt");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello from Azure Function!"));
        await blobClient.UploadAsync(stream, overwrite: true);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("File uploaded to Blob Storage!"); // <- async
        return response;
    }
}
