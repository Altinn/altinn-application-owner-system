using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AltinnApplicationOwnerSystem.Functions
{
    /// <summary>
    /// Azure Function that confirmes that data for a given instance is downloaded
    /// </summary>
    public class EventsConfirmation
    {
        private readonly IAltinnApp _altinnApp;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsConfirmation"/> class.
        /// </summary>
        public EventsConfirmation(IAltinnApp altinnApp)
        {
            _altinnApp = altinnApp;
        }

        /// <summary>
        /// Function method that is triggered
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Function("EventsConfirmation")]
        public async Task Run([QueueTrigger("events-confirmation", Connection = "QueueStorage")]string item, FunctionContext executionContext)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);

            await _altinnApp.AddCompleteConfirmation(cloudEvent.Source.AbsoluteUri);
        }
    }
}
