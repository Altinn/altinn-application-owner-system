using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Models;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AltinnApplicationOwnerSystem.Functions.Services.Implementation
{
    /// <summary>
    /// The queue service that handles actions related to the queue storage.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class QueueService : IQueueService
    {
        private readonly QueueStorageSettings _settings;

        private QueueClient _inboundQueueClient;

        private QueueClient _confirmationQueueClient;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueService"/> class.
        /// </summary>
        /// <param name="settings">The queue storage settings</param>
        public QueueService(IOptions<QueueStorageSettings> settings, ILogger<QueueService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<PushQueueReceipt> PushToInboundQueue(string content)
        {
            try
            {
                QueueClient client = await GetInboundQueueClient();
                await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(content)));
            }
            catch (Exception e)
            {   
                return new PushQueueReceipt { Success = false, Exception = e };
            }

            return new PushQueueReceipt { Success = true };
        }

        /// <inheritdoc/>
        public async Task<PushQueueReceipt> PushToConfirmationQueue(string content)
        {
            try
            {
                QueueClient client = await GetConfirmationQueueClient();
                await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(content)));
            }
            catch (Exception e)
            {
                return new PushQueueReceipt { Success = false, Exception = e };
            }

            return new PushQueueReceipt { Success = true };
        }

        private async Task<QueueClient> GetInboundQueueClient()
        {
            if (_inboundQueueClient == null)
            {
                _inboundQueueClient = new QueueClient(_settings.ConnectionString, _settings.InboundQueueName);
                await _inboundQueueClient.CreateIfNotExistsAsync();
            }

            return _inboundQueueClient;
        }

        private async Task<QueueClient> GetConfirmationQueueClient()
        {
            if (_confirmationQueueClient == null)
            {
                _confirmationQueueClient = new QueueClient(_settings.ConnectionString, _settings.ConfirmationQueueName);
                await _confirmationQueueClient.CreateIfNotExistsAsync();
            }

            return _confirmationQueueClient;
        }
    }
}
