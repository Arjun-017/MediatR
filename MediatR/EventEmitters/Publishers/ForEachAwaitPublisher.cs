using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;

namespace MediatR.EventEmitters.Publishers;

public class ForEachAwaitPublisher : INotificationPublisher
{
    public async Task PublishAsync(IEnumerable<NotificationHandlerExecutor> executors, INotification notification, CancellationToken token)
    {
        var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(token);   
        foreach (var executor in executors)
        {
            if (cancellationSource.IsCancellationRequested) return;

            await executor.HandlerCallback(notification, token);
        }
    }
}
