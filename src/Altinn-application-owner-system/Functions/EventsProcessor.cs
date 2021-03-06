using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AltinnApplicationOwnerSystem.Functions
{
    /// <summary>
    /// Azure Function responsible for downloading data for a given instance.
    /// Triggered by CloudEvent on Azure Queue
    /// When finished it forward CloudEvent to confirmation queue
    /// </summary>
    public class EventsProcessor
    {
        private readonly IAltinnApp _altinnApp;

        private readonly IPlatform _platform;

        private readonly IStorage _storage;

        private readonly IQueueService _queueService;

        private static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsProcessor"/> class.
        /// </summary>
        public EventsProcessor(
            IAltinnApp altinnApp, 
            IPlatform platform, 
            IStorage storage,
            IQueueService queueService,
            ILogger<EventsProcessor> logger)
        {
            _altinnApp = altinnApp;
            _platform = platform;
            _storage = storage;
            _queueService = queueService;
            _logger = logger;
        }

        /// <summary>
        /// Reads cloud event from events-inbound queue and download instance and data for that given event and store it to configured azure storage
        /// </summary>
        [Function(nameof(EventsProcessor))]
        public async Task Run([Microsoft.Azure.Functions.Worker.QueueTrigger("events-inbound", Connection = "QueueStorageSettings:ConnectionString")] string item, FunctionContext executionContext)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);
            
            if (!ShouldProcessEvent(cloudEvent))
            {
                return;
            }

            (string appId, string instanceId) appInfo = GetInstanceInfoFromSource(cloudEvent.Source);
            Instance instance = await _altinnApp.GetInstance(appInfo.appId, appInfo.instanceId);

            string instanceGuid = instance.Id.Split("/")[1];
            string instanceFolder = instance.AppId + "/" + instanceGuid + "/";
            string instancePath = instanceFolder + instanceGuid;
            await _storage.SaveBlob(instancePath, JsonSerializer.Serialize(instance));
            foreach (DataElement data in instance.Data)
            {
                ResourceLinks links = data.SelfLinks;
                using (Stream stream = await _platform.GetBinaryData(links.Platform))
                {
                    await _storage.UploadFromStreamAsync(data.BlobStoragePath, stream);
                }
            }

            await _queueService.PushToFeedbackQueue(JsonSerializer.Serialize(cloudEvent));
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

        /// <summary>
        ///  Will based on configuration decide if the event need to be processed. Todo add logic
        /// </summary>
        private bool ShouldProcessEvent(CloudEvent cloudEvent)
        {
            if (cloudEvent.Type == "platform.events.validatesubscription") 
            {
                return false;
            }

            return true;
        }
    }
}
