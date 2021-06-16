using System;
using System.IO;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface for data handling
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Gets the data as is.
        /// </summary>
        /// <param name="dataUri">Uri to the data</param>
        Task<Stream> GetBinaryData(string dataUri);
    }
}
