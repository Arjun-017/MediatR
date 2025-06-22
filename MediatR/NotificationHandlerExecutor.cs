using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;

namespace MediatR;

public struct NotificationHandlerExecutor(object HandlerInstance, Func<INotification, CancellationToken, Task> HandlerCallback)
{
    public object HandlerInstance { get; } = HandlerInstance;
    public Func<INotification, CancellationToken, Task> HandlerCallback { get; } = HandlerCallback;
}
