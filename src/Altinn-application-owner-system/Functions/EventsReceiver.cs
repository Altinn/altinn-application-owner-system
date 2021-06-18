using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions
{
    /// <summary>
    /// This function is responsible for receving events from Altinn Events.
    /// It will store events in the incomming que for processing by the EventsProcessor
    /// </summary>
    public  class EventsReceiver
    {
        private readonly IQueueService _queueService;

        private readonly AltinnApplicationOwnerSystemSettings _settings;

        public EventsReceiver(IQueueService queueSerice, IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings)
        {
            _settings = altinnIntegratorSettings.Value;
            _queueService = queueSerice;
        }

        [Function("EventsReceiver")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {

            var logger = executionContext.GetLogger("EventsReceiver");

            logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = "";

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CloudEvent cloudEvent = JsonSerializer.Deserialize<CloudEvent>(requestBody);
           

            await _queueService.PushToInboundQueue(JsonSerializer.Serialize(cloudEvent));

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(responseMessage);
            return response;
        }
    }
}

