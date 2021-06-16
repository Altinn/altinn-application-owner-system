using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AltinnApplicationOwnerSystem.Functions
{
    public  class EventsConfirmation
    {
        private readonly IAltinnApp _altinnApp;

        public EventsConfirmation(IAltinnApp altinnApp)
        {
            _altinnApp = altinnApp;
        }

        [FunctionName("EventsConfirmation")]
        public async Task Run([QueueTrigger("events-confirmation", Connection = "QueueStorage")]string item, ILogger log)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);

            await _altinnApp.AddCompleteConfirmation(cloudEvent.Source.AbsoluteUri);

            log.LogInformation($"C# Queue trigger function processed: {item}");
        }
    }
}
