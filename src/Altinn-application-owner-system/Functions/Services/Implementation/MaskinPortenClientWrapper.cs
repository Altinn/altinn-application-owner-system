using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// HttpClient wrapper responsible for calling the MaskinPorten endpont to authenticate the Application Owner System
    /// </summary>
    public class MaskinportenClientWrapper : IMaskinPortenClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        private readonly AltinnApplicationOwnerSystemSettings _settings;

        private readonly HttpClient _client;

        private static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskinportenClientWrapper" /> class.
        /// </summary>
        public MaskinportenClientWrapper(IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings, HttpClient httpClient, ILogger<MaskinportenClientWrapper> logger)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.BaseAddress = new Uri(_settings.MaskinportenBaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        /// <inheritdoc/>
        public async Task<string> PostToken(FormUrlEncodedContent bearer)
        {
            string token = string.Empty;

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_settings.MaskinportenBaseAddress + "/token"),
                Content = bearer
            };

            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                token = response.Content.ReadAsStringAsync().Result;
                return token;
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                _logger.LogError(@"Could not retrieve Token" + error);
            }

            return null;
        }
    }
}
