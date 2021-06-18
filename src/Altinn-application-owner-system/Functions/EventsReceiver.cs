using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions
{
    /// <summary>
    /// This function is responsible for receving events from Altinn Events.
    /// It will store events in the incomming que for processing by the EventsProcessor
    /// </summary>
    public class EventsReceiver
    {
        private readonly IQueueService _queueService;

        private readonly AltinnApplicationOwnerSystemSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsReceiver"/> class.
        /// </summary>
        public EventsReceiver(IQueueService queueSerice, IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings)
        {
            _settings = altinnIntegratorSettings.Value;
            _queueService = queueSerice;
        }

        /// <summary>
        /// Webhook method to receive CloudEvents from Altinn Platform Events
        /// </summary>
        [Function("EventsReceiver")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CloudEvent cloudEvent = JsonSerializer.Deserialize<CloudEvent>(requestBody);

            await _queueService.PushToInboundQueue(JsonSerializer.Serialize(cloudEvent));

            string responseMessage = "Cloud Event received and pushed to processing queue";

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(responseMessage);
            return response;
        }
    }
}