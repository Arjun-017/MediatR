using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;

namespace MediatR.EventEmitters.Publishers;

public class TaskWhenAllPublisher : INotificationPublisher
{
    public async Task PublishAsync(IEnumerable<NotificationHandlerExecutor> executors, INotification notification, CancellationToken token)
    {
        var executionTasks = executors.Select(x => x.HandlerCallback(notification, token));

        await Task.WhenAll(executionTasks);
    }
}
