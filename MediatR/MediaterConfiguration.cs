using System.Reflection;
using MediatR.Abstractions;

namespace MediatR;

public class MediaterConfiguration
{
    public Assembly SourceAssembly { get; set; }
    public INotificationPublisher NotificationPublisher { get; set; }
}
