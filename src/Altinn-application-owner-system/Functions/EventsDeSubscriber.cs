using System.Text.Json;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AltinnApplicationOwnerSystem.Functions
{
    /// <summary>
    /// Azure Function that confirmes that data for a given instance is downloaded
    /// </summary>
    public class EventsDeSubscriber
    {
        private readonly ISubscription _subscriptionService;

        private static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFeedback"/> class.
        /// </summary>
        public EventsDeSubscriber(ISubscription subscriptionService, ILogger<EventsSubscriber> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Function method that removes event subscription when file is added to blob storage
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operationŒ.</returns>
        [Function("EventsDeSubscriber")]
        public async Task Run(
            [BlobTrigger("remove-subscriptions/{name}.json", Connection = "QueueStorageSettings:ConnectionString")] byte[] blob, // Use byte[] https://github.com/Azure/azure-functions-dotnet-worker/issues/398
            string name)
        {
            string blobContent = System.Text.Encoding.UTF8.GetString(blob);
            _logger.LogInformation($"C# Blob trigger function Processed blob Name: {name} Content: {blobContent}");
            try
            {
                Subscription subscription = JsonSerializer.Deserialize<Subscription>(blobContent);
                _logger.LogInformation($"Deserialized object as json: {subscription.ToJson()}");
                await _subscriptionService.RemoveSubscription(name, subscription.Id);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blob content");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while registering subscription");
            }
        }
    }
}