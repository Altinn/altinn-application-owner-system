namespace AltinnApplicationOwnerSystem.Functions.Config
{
    /// <summary>
    /// Configuration object used to hold settings for the queue storage.
    /// </summary>
    public class QueueStorageSettings
    {
        /// <summary>
        /// ConnectionString for the storage account
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the queue to push incomming events to.
        /// </summary>
        public string InboundQueueName { get; set; } = "events-inbound";

        /// <summary>
        /// Name of the queue to push confirmation
        /// </summary>
        public string ConfirmationQueueName { get; set; } = "events-confirmation";

        /// <summary>
        /// Name of the queue to push feedback
        /// </summary>
        public string FeedbackQueueName { get; set; } = "events-feedback";
    }
}
