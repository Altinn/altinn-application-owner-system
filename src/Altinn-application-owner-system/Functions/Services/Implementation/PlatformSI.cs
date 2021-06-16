using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Extensions;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// Service that downloads data from platform
    /// </summary>
    public class PlatformSI : IPlatform
    {
        private readonly HttpClient _client;
        private readonly AltinnApplicationOwnerSystemSettings _settings;
        private readonly IAuthenticationService _authenticationService;

        public PlatformSI(IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings,
            HttpClient httpClient, IAuthenticationService authenticationService)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
            _authenticationService = authenticationService;
        }


        public async Task<Stream> GetBinaryData(string dataUri)
        {
            string altinnToken = await _authenticationService.GetAltinnToken();

            HttpResponseMessage response = await _client.GetAsync(altinnToken, dataUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new ApplicationException();
        }
    }
}
