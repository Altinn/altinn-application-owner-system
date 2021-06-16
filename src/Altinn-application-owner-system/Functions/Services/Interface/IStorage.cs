using System.IO;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    public interface IStorage
    {
        public Task SaveBlob(string name, string data);

        public Task<long> UploadFromStreamAsync(Stream stream, string fileName);
    }
}
