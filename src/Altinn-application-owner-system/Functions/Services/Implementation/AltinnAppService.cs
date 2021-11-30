using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Extensions;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// App implementation of the instance service that talks to the given app
    /// </summary>
    public class AltinnAppService : IAltinnApp
    {
        private readonly ILogger _logger;

        private readonly HttpClient _client;

        private readonly AltinnApplicationOwnerSystemSettings _settings;

        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AltinnAppService"/> class.
        /// </summary>
        public AltinnAppService(
            IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings,
            HttpClient httpClient,
            IAuthenticationService authenticationService,
            ILogger<AltinnAppService> logger)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Instance> GetInstance(string appId, string instanceId)
        {
            string apiUrl = $"{_settings.AppsBaseUrl}{appId}instances/{instanceId}";

            string altinnToken = await _authenticationService.GetAltinnToken();

            HttpResponseMessage response = await _client.GetAsync(altinnToken, apiUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string instanceData = await response.Content.ReadAsStringAsync();
                Instance instance = JsonConvert.DeserializeObject<Instance>(instanceData);
                return instance;
            }
            else
            {
                _logger.LogError($"Unable to fetch instance with instance id {instanceId} " + response.StatusCode + " " + apiUrl);
                throw new ApplicationException();
            }
        }

        /// <inheritdoc/>
        public async Task<Instance> AddCompleteConfirmation(string instanceUri)
        {
            string apiUrl = $"{instanceUri}/complete";
            string altinnToken = await _authenticationService.GetAltinnToken();

            HttpResponseMessage response = await _client.PostAsync(altinnToken, apiUrl, new StringContent(string.Empty));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string instanceData = await response.Content.ReadAsStringAsync();
                Instance instance = JsonConvert.DeserializeObject<Instance>(instanceData);
                return instance;
            }

            throw new ApplicationException();
        }

        /// <inheritdoc/>
        public async Task<Instance> AddXMLFeedback(string instanceUri, string instanceId, string eventType)
        {
            string xmlFeedBackDataType = _settings.XMLFeedbackDataType;
            string feedbackOnEventType = _settings.XMLFeedbackEventType;
            if (!string.IsNullOrEmpty(xmlFeedBackDataType) && !string.IsNullOrEmpty(feedbackOnEventType) && feedbackOnEventType.Equals(eventType))
            {
                string apiUrl = $"{instanceUri}/data?dataType={xmlFeedBackDataType}";
                string altinnToken = await _authenticationService.GetAltinnToken();
                var contentData = new StringContent(GenerateFeedbackXML(instanceId), Encoding.UTF8, "text/xml");
                contentData.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                contentData.Headers.TryAddWithoutValidation("Content-Disposition", "attachment; filename=test.xml");
                HttpResponseMessage response = await _client.PostAsync(altinnToken, apiUrl, contentData);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string instanceData = await response.Content.ReadAsStringAsync();
                    Instance instance = JsonConvert.DeserializeObject<Instance>(instanceData);
                    return instance;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Unable to add xml feedback with instance id {instanceId} {response.StatusCode} message {response.ReasonPhrase} content {responseContent}");
                }

                throw new ApplicationException();
            }

            return new Instance();
        }

        private string GenerateFeedbackXML(string instanceId)
        {
            // xmlns=\"http://altinn.no/services/ws/schemas/ServiceEngine/InstanceData/1.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://altinn.no/services/ws/schemas/ServiceEngine/InstanceData/1.0 http://altinn.no/services/ws/schemas/ServiceEngine/InstanceData/1.0/InstanceData.xsd\"
            string xml = $"<feedback>" +
                $"<instanceId>{instanceId}</instanceId>" +
                $"<feedbackText>This is a feedback</feedbackText>" +
                $"<feedbackType>1</feedbackType>" +
                $"<feedbackDate>{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}</feedbackDate>" +
                $"<feedbackAuthor>aos-ttd</feedbackAuthor>" +
                $"</feedback>";
            return xml;
        }
    }
}
