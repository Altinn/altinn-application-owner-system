using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AltinnApplicationOwnerSystem.Functions
{
    public class EventsProcessor
    {
        private readonly IAltinnApp _altinnApp;

        private readonly IPlatform _platform;

        private readonly IStorage _storage;

        private readonly IQueueService _queueService;

        public EventsProcessor(
            IAltinnApp altinnApp, 
            IPlatform platform, 
            IStorage storage,
            IQueueService queueService)
        {
            _altinnApp = altinnApp;
            _platform = platform;
            _storage = storage;
            _queueService = queueService;
        }

        /// <summary>
        /// Reads cloud event from events-inbound queue and download instance and data for that given event and store it to configured azure storage
        /// </summary>
        [FunctionName("EventsProcessor")]
        public async Task Run([QueueTrigger("events-inbound", Connection = "QueueStorage")] string item, ILogger log)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);
            if (ShouldProcessEvent(cloudEvent))
            {
                Instance instance = CreateInstanceFromSource(cloudEvent);
                instance = await _altinnApp.GetInstance(instance.AppId, instance.Id);

                string instanceGuid = instance.Id.Split("/")[1];
                string instancePath = instance.AppId + "/" + instanceGuid + "/" + instanceGuid;
                await _storage.SaveBlob(instancePath, JsonSerializer.Serialize(instance));
                foreach (DataElement data in instance.Data)
                {
                    ResourceLinks links = data.SelfLinks;
                    using (Stream stream = await _platform.GetBinaryData(links.Platform))
                    {
                        await _storage.UploadFromStreamAsync(stream, data.BlobStoragePath);
                    }
                }

                await _queueService.PushToConfirmationQueue(JsonSerializer.Serialize(cloudEvent));
            }
        }

        /// <summary>
        /// Creates an instance for a given event
        /// </summary>
        private Instance CreateInstanceFromSource(CloudEvent cloudEvent)
        {
            Instance instance = new Instance();
            string[] parts =  cloudEvent.Source.PathAndQuery.Split("/");
            instance.AppId = $"{parts[1]}/{parts[2]}";
            instance.Org = $"{parts[1]}";
            instance.InstanceOwner = new InstanceOwner() { PartyId = parts[4] };
            instance.Id = $"{parts[4]}/{parts[5]}";
            return instance;
        }

        /// <summary>
        ///  Will based on configuration decide if the event need to be processed. Todo add logic
        /// </summary>
        private bool ShouldProcessEvent(CloudEvent cloudEvent)
        {
            return true;
        }
    }
}
