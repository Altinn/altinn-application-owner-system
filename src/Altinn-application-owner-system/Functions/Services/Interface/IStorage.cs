using System.IO;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface for Storage where Application Owner system store received data
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Saves a specific blob
        /// </summary>
        Task SaveBlob(string name, string data);

        /// <summary>
        /// Saves a stream to blob
        /// </summary>
        Task<long> UploadFromStreamAsync(string name, Stream stream);

        /// <summary>
        /// Save registered subscription information
        /// </summary>
        Task SaveRegisteredSubscription(string name, Subscription subscription);

        /// <summary>
        /// Delete blob from container
        /// </summary>
        Task DeleteBlobFromContainer(string name, string container);
    }
}
