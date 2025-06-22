using System;
using MediatR.Abstractions;
using MediatR.Publishers;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR;

public static class IOCExtension
{
    public static IServiceCollection AddMediater(this IServiceCollection services, Action<MediaterConfiguration> configureAction)
    {
        var configuration = new MediaterConfiguration();
        configureAction(configuration);

        services.AddSingleton(configuration);
        services.AddSingleton<IQueryHandlerResolver, QueryHandlerResolver>();

        services.AddSingleton<ISender, RequestEmitter>();
        services.AddSingleton<IPublisher, NotificationEmitter>();

        services.AddSingleton<INotificationPublisher>((sp) => configuration.NotificationPublisher);

        return services;
    } 
}
