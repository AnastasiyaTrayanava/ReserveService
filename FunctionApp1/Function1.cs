using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly BlobContainerClient _blobContainerClient;

		public Function1(ILogger<Function1> logger, BlobContainerClient blobContainerClient)
        {
            _logger = logger;
            _blobContainerClient = blobContainerClient;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
	        try
	        {
		        _logger.LogInformation("C# HTTP trigger function processed a request.");

		        var reqBody = await new StreamReader(req.Body).ReadToEndAsync();

		        var client = _blobContainerClient.GetBlobClient(DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".json");
		        var response = await client.UploadAsync(BinaryData.FromString(reqBody));

		        return new OkObjectResult("It is done! Response: " + response.Value);
	        }
	        catch (Exception e)
	        {
				return new BadRequestObjectResult("Something went wrong. Message: " + e.Message);
			}
        }
    }
}
