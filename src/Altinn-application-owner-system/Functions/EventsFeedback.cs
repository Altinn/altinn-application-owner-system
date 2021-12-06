using System;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;

namespace AltinnApplicationOwnerSystem.Functions
{
    /// <summary>
    /// Azure Function that confirmes that data for a given instance is downloaded
    /// </summary>
    public class EventsFeedback
    {
        private readonly IAltinnApp _altinnApp;

        private readonly IQueueService _queueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFeedback"/> class.
        /// </summary>
        public EventsFeedback(IAltinnApp altinnApp, IQueueService queueService)
        {
            _altinnApp = altinnApp;
            _queueService = queueService;
        }

        /// <summary>
        /// Function method that is triggered by new element on events-feedback queue
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Function("EventsFeedback")]
        public async Task Run([QueueTrigger("events-feedback", Connection = "QueueStorageSettings:ConnectionString")] string item, FunctionContext executionContext)
        {
            CloudEvent cloudEvent = JsonSerializer.Deserialize<CloudEvent>(item);
            (string appId, string instanceId) appInfo = GetInstanceInfoFromSource(cloudEvent.Source);

            await _altinnApp.AddXMLFeedback(cloudEvent.Source.AbsoluteUri, appInfo.instanceId, cloudEvent.Type);
            
            // Push to confirmation queue when Feedback is completed
            await _queueService.PushToConfirmationQueue(JsonSerializer.Serialize(cloudEvent));
        }

        /// <summary>
        /// Creates an instance for a given event
        /// </summary>
        private (string, string) GetInstanceInfoFromSource(Uri eventUri)
        {
            string[] parts = eventUri.Segments;
            (string appId, string instanceId) appInfo = ($"{parts[1]}{parts[2]}", $"{parts[4]}{parts[5]}");
            return appInfo;
        }
    }
}