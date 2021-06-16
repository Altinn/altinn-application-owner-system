using System;
using System.IO;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// Class that handles integration with Azure Blob Storage.
    /// </summary>
    public class StorageSI: IStorage
    {
        private readonly AltinnApplicationOwnerSystemSettings _settings;

        public StorageSI(IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings)
        {
            _settings = altinnIntegratorSettings.Value;
        }

        /// <summary>
        /// Saves data in blob storage defined in configuration.
        /// </summary>
        /// <param name="config">Configuration.</param>
        /// <param name="name">Blob name.</param>
        /// <param name="data">Blob data.</param>
        public async Task SaveBlob(string name, string data)
        {
            BlobClient client;

            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(_settings.AccountName, _settings.AccountKey);
            BlobServiceClient serviceClient = new BlobServiceClient(new Uri(_settings.BlobEndpoint), storageCredentials);
            BlobContainerClient blobContainerClient = serviceClient.GetBlobContainerClient(_settings.StorageContainer);

            client = blobContainerClient.GetBlobClient(name);
            
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            await client.UploadAsync(stream, true);
            stream.Dispose();
        }

        public async Task<long> UploadFromStreamAsync(Stream stream, string fileName)
        {
            BlobClient blockBlob = CreateBlobClient(fileName);

            await blockBlob.UploadAsync(stream, true);
            BlobProperties properties = await blockBlob.GetPropertiesAsync();

            return properties.ContentLength;
        }

        private BlobClient CreateBlobClient(string blobName)
        {
            BlobClient client;

            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(_settings.AccountName, _settings.AccountKey);
            BlobServiceClient serviceClient = new BlobServiceClient(new Uri(_settings.BlobEndpoint), storageCredentials);
            BlobContainerClient blobContainerClient = serviceClient.GetBlobContainerClient(_settings.StorageContainer);

            client = blobContainerClient.GetBlobClient(blobName);

            return client;
        }
    }
}
