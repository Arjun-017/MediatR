using System;
using System.Reflection;
using static MediatR.QueryHandlerResolver;

namespace MediatR.Abstractions
{
    internal interface IQueryHandlerResolver
    {
        void MapQueryToHandler(Assembly executingAssembly);
        Type GetQueryHandlerType(Type query);
        QueryHandlerExecutionDelegate GetQueryHandlerExecutionDelegate(Type queryHandlerType);
    }
}
