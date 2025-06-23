using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;
using MediatR.EventEmitters;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Publishers;

internal class NotificationEmitter : BaseEmitter, IPublisher
{
    
    private readonly IQueryHandlerResolver _queryHandlerResolver;
    private readonly INotificationPublisher _notificationPublisher;
    
    public NotificationEmitter(IServiceScopeFactory serviceScopeFactory, IQueryHandlerResolver queryHandlerResolver,
        INotificationPublisher notificationPublisher) : base(serviceScopeFactory)
    {
        _queryHandlerResolver = queryHandlerResolver;
        _notificationPublisher = notificationPublisher;
    }

    public async Task SendAsync(INotification notification, CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return;

        var queryType = notification.GetType();
        var handlerTypes = _queryHandlerResolver.GetQueryHandlerTypes(queryType);

        var notificationExecutors = GetNotificationHandlerExecutors(handlerTypes);
        
        await _notificationPublisher.PublishAsync(notificationExecutors, notification, token);
    }

    private IEnumerable<NotificationHandlerExecutor> GetNotificationHandlerExecutors(List<Type> handlerTypes)
    {
        foreach (var handlerType in handlerTypes)
        {
            var handler = GetHandlerInstance(handlerType);
            var handlerDelegate = _queryHandlerResolver.GetQueryHandlerExecutionDelegate(handlerType);
            Func<INotification, CancellationToken, Task> executorCallback = (notificationInstance, cancellationToken) =>
            {
                return handlerDelegate(handler, notificationInstance, cancellationToken);
            };
            yield return new NotificationHandlerExecutor(handler, executorCallback);
        }
    }

}
