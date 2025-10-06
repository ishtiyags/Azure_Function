using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public class TableFunction
{
    private readonly ILogger _logger;

    public TableFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TableFunction>();
    }

    [Function("InsertTableEntity")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        var tableClient = tableServiceClient.GetTableClient("CustomerInfo");
        await tableClient.CreateIfNotExistsAsync();

        var customer = new TableEntity("Customers", Guid.NewGuid().ToString())
        {
            {"Name", "John Doe"},
            {"Email", "johndoe@example.com"}
        };

        await tableClient.AddEntityAsync(customer);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Customer added to Table Storage!");  // ? Async
        return response;
    }
}
