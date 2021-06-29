using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Extensions
{
    /// <summary>
    /// This extension is created to make it easy to add a bearer token to a HttpRequests. 
    /// </summary>
    public static class HttpClientExtension
    {
        /// <summary>
        /// Extension that add authorization header to request
        /// </summary>
        /// <returns>A HttpResponseMessage</returns>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string authorizationToken, string requestUri, HttpContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("Authorization", "Bearer " + authorizationToken);
            request.Content = content;
            return httpClient.SendAsync(request, CancellationToken.None);
        }

        /// <summary>
        /// Extension that add authorization header to request
        /// </summary>
        /// <param name="httpClient">The HttpClient</param>
        /// <param name="authorizationToken">the authorization token (jwt)</param>
        /// <param name="requestUri">The request Uri</param>
        /// <returns>A HttpResponseMessage</returns>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string authorizationToken, string requestUri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", "Bearer " + authorizationToken);
            return httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }
    }
}
