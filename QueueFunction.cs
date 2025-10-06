using Azure.Storage.Queues;                  // For creating/writing queues
using Microsoft.Azure.Functions.Worker;      // For Azure Function attributes
using Microsoft.Extensions.Logging;          // For logging

public class QueueFunction
{
    private readonly ILogger _logger;

    public QueueFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<QueueFunction>();
    }

    [Function("ProcessQueueMessage")]
    public void Run([QueueTrigger("transaction-queue", Connection = "AzureWebJobsStorage")] string message)
    {
        _logger.LogInformation($"Received message: {message}");
        // Optionally: Write processed info to Table or Blob
    }
}
