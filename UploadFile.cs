using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public class UploadFileFunction
{
    private readonly ILogger _logger;

    public UploadFileFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UploadFileFunction>();
    }

    [Function("UploadFile")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        // 1. Get connection string
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        if (string.IsNullOrEmpty(connectionString))
        {
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            errorResponse.WriteString("Azure Storage connection string is missing.");
            return errorResponse;
        }

        // 2. Create/get file share
        var shareClient = new ShareClient(connectionString, "sharefiles");
        await shareClient.CreateIfNotExistsAsync();

        // 3. Get root directory
        var root = shareClient.GetRootDirectoryClient();

        // 4. Read file content from request body
        using var stream = new MemoryStream();
        await req.Body.CopyToAsync(stream);
        stream.Position = 0;

        // 5. Use a fixed filename (or read from query string)
        var fileName = req.Url.Query.Contains("filename=")
            ? req.Url.Query.Split("filename=")[1]
            : "uploadedfile.txt";

        var fileClient = root.GetFileClient(fileName);

        await fileClient.CreateAsync(stream.Length);
        await fileClient.UploadRangeAsync(new Azure.HttpRange(0, stream.Length), stream);

        // 6. Respond
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.WriteString($"File '{fileName}' uploaded successfully to Azure Files!");
        _logger.LogInformation($"File '{fileName}' uploaded successfully.");
        return response;
    }
}
