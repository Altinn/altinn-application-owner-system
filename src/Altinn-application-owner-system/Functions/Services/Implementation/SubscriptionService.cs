using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Extensions;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// SubscriptionService implements ISubscription and handles subscriptions against Altinn.
    /// </summary>
    public class SubscriptionService : ISubscription
    {
        private readonly ILogger _logger;
        
        private readonly HttpClient _client;
        
        private readonly AltinnApplicationOwnerSystemSettings _settings;
        
        private readonly IAuthenticationService _authenticationService;

        private readonly IStorage _storage;
        
        /// <summary>
        /// Create a instance of SubscriptionService
        /// </summary>
        public SubscriptionService(
            IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings, 
            HttpClient httpClient, 
            IAuthenticationService authenticationService,
            IStorage storage,
            ILogger<SubscriptionService> logger)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = httpClient;
            _authenticationService = authenticationService;
            _storage = storage;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Subscription> GetSubscription(string subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<Subscription> RegisterSubscription(string name, Subscription subscription)
        {
            string apiUrl = $"{_settings.PlatformBaseUrl}events/api/v1/subscriptions";
            
            string altinnToken = await _authenticationService.GetAltinnToken();
            HttpResponseMessage response = await _client.PostAsync(altinnToken, apiUrl, new StringContent(subscription.ToJson(), Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string subscriptionPath = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"async: {subscriptionPath} Content: {response.Content} Reason: {response.ReasonPhrase}");
                Subscription registered = Subscription.FromJson(subscriptionPath);
                await _storage.SaveRegisteredSubscription($"{name}.json", registered);
                await _storage.DeleteBlobFromContainer($"{name}.json", "add-subscriptions");
                return registered;
            }

            _logger.LogError($"Failed to register subscription. Status code: {response.StatusCode} Phrase: {response.ReasonPhrase} Content: {response.Content}");

            return subscription;
        }

        /// <inheritdoc/>
        public async Task RemoveSubscription(string name, int id)
        {
            string apiUrl = $"{_settings.PlatformBaseUrl}events/api/v1/subscriptions/{id}";
            string altinnToken = await _authenticationService.GetAltinnToken(); 
            HttpResponseMessage response = await _client.DeleteAsync(altinnToken, apiUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await _storage.DeleteBlobFromContainer($"{name}.json", "active-subscriptions");
                await _storage.DeleteBlobFromContainer($"{name}.json", "remove-subscriptions");
                _logger.LogInformation("Subscription removed successfully");
                return;
            }

            _logger.LogError($"Failed to register subscription. Status code: {response.StatusCode}");
        }
    }
}