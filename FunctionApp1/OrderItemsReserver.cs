using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class OrderItemsReserver
    {
        private readonly ILogger<OrderItemsReserver> _logger;
        private readonly BlobServiceClient _blobServiceClient;

		public OrderItemsReserver(BlobServiceClient blobServiceClient, ILogger<OrderItemsReserver> logger)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        [Function("OrderItemsReserver")]
        public async Task Run([ServiceBusTrigger("queue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
	        try
	        {
		        _logger.LogInformation("C# HTTP trigger function processed a request.");

		        var containerClient = _blobServiceClient.GetBlobContainerClient("reserved-items");

				var client = containerClient.GetBlobClient(DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".json");
		        var response = await client.UploadAsync(message.Body);
	        }
	        catch (Exception e)
	        {
		        HttpClient client = new HttpClient();
		        
		        var httpMessage = new HttpRequestMessage()
		        {
			        Method = HttpMethod.Post,
			        RequestUri = new Uri(Environment.GetEnvironmentVariable("AzureBlobConnectionString") ?? string.Empty),
			        Content = new StringContent(JsonSerializer.Serialize(new ErrorBodyMessage
					{
						ErrorMessage = e.Message,
						OrderBody = System.Text.Encoding.Default.GetString(message.Body)
					}))
				};

		        var request = await client.SendAsync(httpMessage);

		        _logger.LogInformation($"Email sent; status code - {request.StatusCode}");
			}
        }
    }
}
