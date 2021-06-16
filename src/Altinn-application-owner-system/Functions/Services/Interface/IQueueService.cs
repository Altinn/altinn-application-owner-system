using AltinnApplicationOwnerSystem.Functions.Models;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface to interact with the queue
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Pushes the provided content to the queue
        /// </summary>
        /// <param name="content">The content to push to the queue in string format</param>
        /// <returns>Returns a queue receipt</returns>
        public Task<PushQueueReceipt> PushToInboundQueue(string content);


        /// <summary>
        /// Pushes the provided content to the queue
        /// </summary>
        /// <param name="content">The content to push to the queue in string format</param>
        /// <returns>Returns a queue receipt</returns>
        public Task<PushQueueReceipt> PushToConfirmationQueue(string content);

    }
}
