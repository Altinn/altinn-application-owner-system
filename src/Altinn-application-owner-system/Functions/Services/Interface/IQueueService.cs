using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Models;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface to interact with the different queues used by the functions
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Pushes the provided content to the queue
        /// </summary>
        /// <param name="content">The content to push to the queue in string format</param>
        /// <returns>Returns a queue receipt</returns>
        Task<PushQueueReceipt> PushToInboundQueue(string content);

        /// <summary>
        /// Pushes the provided content to the queue
        /// </summary>
        /// <param name="content">The content to push to the queue in string format</param>
        /// <returns>Returns a queue receipt</returns>
        Task<PushQueueReceipt> PushToConfirmationQueue(string content);
    }
}
