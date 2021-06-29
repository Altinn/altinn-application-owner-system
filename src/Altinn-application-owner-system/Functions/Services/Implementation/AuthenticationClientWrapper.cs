using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Extensions;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// HttpClient wrapper responsible for calling Altinn Platform Authentication to convert MaskinPorten token to AltinnToken
    /// </summary>
    public class AuthenticationClientWrapper : IAuthenticationClientWrapper
    {
        private readonly HttpClient _client;

        private readonly AltinnApplicationOwnerSystemSettings _settings;

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationClientWrapper" /> class.
        /// </summary>
        public AuthenticationClientWrapper(IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings, HttpClient httpClient)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
        }

        /// <inheritdoc/>
        public async Task<string> ConvertToken(string token)
        {
            string cmd = $@"{_settings.PlatformBaseUrl}authentication/api/v1/exchange/maskinporten?test={_settings.TestMode}";
            HttpResponseMessage response = await _client.GetAsync(token, cmd);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<string>(jsonString);
            }
            else
            {
                return $@"Could not retrieve Altinn Token";
            }
        }
    }
}
