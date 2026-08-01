using PlatformService.Models;

namespace PlatformService.AsyncDataServices
{
    /// <summary>
    /// Abstraction for publishing messages to a message bus.
    /// Implementations should handle connection, serialization and delivery.
    /// </summary>
    public interface IMessageBusClient
    {
        /// <summary>
        /// Publish a platform-created event payload to the bus.
        /// </summary>
        /// <param name="platformPublishedDto">Platform event payload as a DTO.</param>
        Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto);
    }
}
