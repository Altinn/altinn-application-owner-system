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
    public class StorageService : IStorage
    {
        private readonly AltinnApplicationOwnerSystemSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageService"/> class.
        /// </summary>
        public StorageService(IOptions<AltinnApplicationOwnerSystemSettings> altinnIntegratorSettings)
        {
            _settings = altinnIntegratorSettings.Value;
        }

        /// <inheritdoc />
        public async Task DeleteBlobFromContainer(string name, string container)
        {
            BlobClient client;

            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(_settings.AccountName, _settings.AccountKey);
            BlobServiceClient serviceClient = new BlobServiceClient(new Uri(_settings.BlobEndpoint), storageCredentials);
            BlobContainerClient blobContainerClient = serviceClient.GetBlobContainerClient(container);
            client = blobContainerClient.GetBlobClient(name);

            await client.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Saves data in blob storage defined in configuration.
        /// </summary>
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

        /// <inheritdoc />
        public async Task SaveRegisteredSubscription(string name, Subscription subscription)
        {
            BlobClient client;

            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(_settings.AccountName, _settings.AccountKey);
            BlobServiceClient serviceClient = new BlobServiceClient(new Uri(_settings.BlobEndpoint), storageCredentials);
            BlobContainerClient blobContainerClient = serviceClient.GetBlobContainerClient(_settings.RegisteredSubStorageContainer);

            client = blobContainerClient.GetBlobClient(name);

            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(subscription.ToJson());
            writer.Flush();
            stream.Position = 0;
            await client.UploadAsync(stream, true);
            stream.Dispose();
        }

        /// <inheritdoc/>
        public async Task<long> UploadFromStreamAsync(string name, Stream stream)
        {
            BlobClient blockBlob = CreateBlobClient(name);

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
