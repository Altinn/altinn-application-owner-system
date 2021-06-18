using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;
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

        [Function("EventsConfirmation")]
        public async Task Run([QueueTrigger("events-confirmation", Connection = "QueueStorage")]string item, FunctionContext executionContext)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);

            await _altinnApp.AddCompleteConfirmation(cloudEvent.Source.AbsoluteUri);
        }
    }
}
