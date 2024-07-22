namespace Notifier.Lambda.Infrastructure.Redis
{
    /// <summary>
    /// Defines the contract for a Redis-based service used for tracking message processing.
    /// This interface ensures that events are not processed multiple times and supports scenarios where message replay is needed.
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Checks whether the message with the specified ID has already been processed.
        /// This method helps prevent the reprocessing of the same event, ensuring idempotency.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to check.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean result indicating whether the message has been processed.</returns>
        Task<bool> HasBeenProcessedAsync(string messageId);

        /// <summary>
        /// Marks the message with the specified ID as processed.
        /// This method records that the message has been handled, preventing future duplicate processing.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to mark as processed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAsProcessedAsync(string messageId);
    }
}