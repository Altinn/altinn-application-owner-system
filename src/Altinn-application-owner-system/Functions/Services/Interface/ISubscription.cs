using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface for managing event subscriptions
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Register a new subscription
        /// </summary>
        /// <param name="name">Name of the blob</param>
        /// <param name="subscription">The subscription to register</param>
        /// <returns>The registered subscription</returns>
        Task<Subscription> RegisterSubscription(string name, Subscription subscription);

        /// <summary>
        /// Get a subscription by id
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription</param>
        /// <returns>The subscription</returns>
        Task<Subscription> GetSubscription(string subscriptionId);

        /// <summary>
        /// Delete a subscription by id
        /// </summary>
        /// <param name="name">Name of the blob</param>
        /// <param name="subscriptionId">The id of the subscription</param>
        /// <returns>The deleted subscription</returns>
        Task RemoveSubscription(string name, int subscriptionId); 
    }
}