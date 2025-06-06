using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Abstractions
{
    /// <summary>
    /// The handler for IQuery.
    /// </summary>
    /// <typeparam name="TQuery">Event of type IQuery</typeparam>
    public interface IQueryHandler<in TQuery> where TQuery : IQuery
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="query">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task HandleAsync(IQuery query, CancellationToken token = default);
    }

    /// <summary>
    /// The handler for IQuery<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="TQuery">Event of type IQuery<typeparamref name="T"/></typeparam>
    public interface IQueryHandler<in TQuery, T> where TQuery : IQuery<T> where T : class
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="query">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task<T> HandleAsync(IQuery<T> query, CancellationToken token = default);
    }

    /// <summary>
    /// The handler for INotification.
    /// </summary>
    /// <typeparam name="TNotification">Event of type INotification</typeparam>
    public interface INotificationHandler<in TNotification> where TNotification : INotification
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="notification">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task HandleAsync(INotification notification, CancellationToken token = default);
    }

    /// <summary>
    /// The handler for INotification<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="TNotification">Event of type INotification<typeparamref name="T"/></typeparam>
    public interface INotificationHandler<in TNotification, T> where TNotification : INotification<T> where T : class
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="notification">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task<T> HandleAsync(INotification<T> notification, CancellationToken token = default);
    }
}
