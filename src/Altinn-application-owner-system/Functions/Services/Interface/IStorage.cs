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
        public Task SaveBlob(string name, string data);

        /// <summary>
        /// Saves a stream to blob
        /// </summary>
        public Task<long> UploadFromStreamAsync(string name, Stream stream);
    }
}
