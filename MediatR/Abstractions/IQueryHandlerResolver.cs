using System;
using System.Reflection;
using static MediatR.QueryHandlerResolver;

namespace MediatR.Abstractions
{
    internal interface IQueryHandlerResolver
    {
        void AddAssemblyTypes(Assembly executingAssembly);
        Type GetQueryHandlerType(Type queryType, Type returnType = default);
        QueryHandlerExecutionDelegate GetQueryHandlerExecutionDelegate(Type queryHandlerType);
        QueryHandlerExecutionDelegateWithResult<TResult> GetQueryHandlerExecutionDelegateWithResult<TResult>(Type queryHandlerType);
    }
}
