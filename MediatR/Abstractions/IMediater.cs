using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Abstractions
{
    /// <summary>
    /// ISender and IOublisher together. Use this to send/publish events.
    /// </summary>
    public interface IMediater : ISender, INotification { }

    /// <summary>
    /// Use this to send events that will be handled by a single handler that takes IQuery input.
    /// </summary>
    public interface ISender {
        /// <summary>
        /// Sends an event
        /// </summary>
        /// <param name="query">The event that extends IQuery</param>
        /// <param name="token">CancellationToken token</param>
        Task SendAsync(IQuery query, CancellationToken token = default);
        /// <summary>
        /// Sends an event
        /// </summary>
        /// <param name="query">The event that extends IQuery<typeparamref name="T"/></param>
        /// <param name="token">CancellationToken token</param>
        Task<T> SendAsync<T>(IQuery<T> query, CancellationToken token = default);
    }

    /// <summary>
    /// Use this to send events that will be handled all the handlers that takes INotification input.
    /// </summary>
    public interface IPublisher {
        /// <summary>
        /// Sends an event to all the handlers
        /// </summary>
        /// <param name="notification">The event that extends INotification</param>
        /// <param name="token">CancellationToken token</param>
        Task SendAsync(INotification notification, CancellationToken token = default);
        /// <summary>
        /// Sends an event to all the handlers
        /// </summary>
        /// <param name="notification">The event that extends INotification<typeparamref name="T"/></param>
        /// <param name="token">CancellationToken token</param>
        Task<T> SendAsync<T>(INotification<T> query, CancellationToken token = default);
    }
}
