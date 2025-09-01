using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddAzureClients(clientBuilder =>
{
	clientBuilder.AddClient<BlobServiceClient, BlobClientOptions>((clientOptions, tokenCredential, serviceProvider) =>
	{
		BlobClientOptions blobOptions = new BlobClientOptions()
		{
			Retry =
			{
				Delay = TimeSpan.FromSeconds(2),
				MaxRetries = 3,
				Mode = RetryMode.Exponential,
				MaxDelay = TimeSpan.FromSeconds(10),
				NetworkTimeout = TimeSpan.FromSeconds(100)
			},
		};
		var serviceUri = new Uri(Environment.GetEnvironmentVariable("AzureBlobConnectionString") ?? string.Empty);

		return new BlobServiceClient(serviceUri, new DefaultAzureCredential(), blobOptions);
	});
});

builder.Build().Run();
