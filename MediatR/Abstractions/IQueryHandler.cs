using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Abstractions
{

    public interface IBaseHandler { };
    /// <summary>
    /// The handler for IQuery.
    /// </summary>
    /// <typeparam name="TQuery">Event of type IQuery</typeparam>
    public interface IQueryHandler<in TQuery> : IBaseHandler where TQuery : IQuery
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="query">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task HandleAsync(TQuery query, CancellationToken token = default);
    }

    /// <summary>
    /// The handler for IQuery<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="TQuery">Event of type IQuery<typeparamref name="T"/></typeparam>
    public interface IQueryHandler<in TQuery, T> : IBaseHandler where TQuery : IQuery<T>
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="query">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task<T> HandleAsync(TQuery query, CancellationToken token = default);
    }

    /// <summary>
    /// The handler for INotification.
    /// </summary>
    /// <typeparam name="TNotification">Event of type INotification</typeparam>
    public interface INotificationHandler<in TNotification> : IBaseHandler where TNotification : INotification
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="notification">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task HandleAsync(TNotification notification, CancellationToken token = default);
    }

    /// <summary>
    /// The handler for INotification<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="TNotification">Event of type INotification<typeparamref name="T"/></typeparam>
    public interface INotificationHandler<in TNotification, T> : IBaseHandler where TNotification : INotification<T>
    {
        /// <summary>
        /// Handler method
        /// </summary>
        /// <param name="notification">Event to be handled</param>
        /// <param name="token">CancellationToken token</param>
        Task<T> HandleAsync(TNotification notification, CancellationToken token = default);
    }
}
