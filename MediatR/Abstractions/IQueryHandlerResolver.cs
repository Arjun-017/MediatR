using System;
using System.Collections.Generic;
using static MediatR.QueryHandlerResolver;

namespace MediatR.Abstractions;

internal interface IQueryHandlerResolver
{
    Type GetQueryHandlerType(Type queryType);
    List<Type> GetQueryHandlerTypes(Type queryType);
    QueryHandlerExecutionDelegate GetQueryHandlerExecutionDelegate(Type queryHandlerType);
    QueryHandlerExecutionDelegateWithResult<TResult> GetQueryHandlerExecutionDelegateWithResult<TResult>(Type queryHandlerType);
}
