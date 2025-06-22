using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.EventEmitters;

internal class BaseEmitter
{
        
    protected readonly IServiceScopeFactory _serviceScopeFactory;
        
    public BaseEmitter(IServiceScopeFactory serviceScopeFactory) {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected object GetHandlerInstance(Type handlerType)
    {
        var dependentTypes = GetInjectedServiceTypes(handlerType);
        var dependentObjects = new object[dependentTypes.Length];

        var scope = _serviceScopeFactory.CreateScope();
        int index = 0;
        foreach (var dependentType in dependentTypes)
        {
            var dependent = scope.ServiceProvider.GetRequiredService(dependentType);
            dependentObjects[index] = dependent;
            index += 1;
        }
        scope.Dispose();

        var handler = Activator.CreateInstance(handlerType, dependentObjects);
        return handler;
    }

    protected Type[] GetInjectedServiceTypes(Type targetType)
    {
        var constructor = targetType
            .GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
            return Array.Empty<Type>();

        return constructor
            .GetParameters()
            .Select(p => p.ParameterType)
            .ToArray();
    }
}

