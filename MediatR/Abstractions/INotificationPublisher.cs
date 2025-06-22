using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Abstractions;

public interface INotificationPublisher
{
    Task PublishAsync(IEnumerable<NotificationHandlerExecutor> executors, INotification notification, CancellationToken token);
}
