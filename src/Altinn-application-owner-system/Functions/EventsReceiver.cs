using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Implementation;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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

        [FunctionName("EventsReceiver")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
           
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CloudEvent cloudEvent = JsonSerializer.Deserialize<CloudEvent>(requestBody);
           

            await _queueService.PushToInboundQueue(JsonSerializer.Serialize(cloudEvent));

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

