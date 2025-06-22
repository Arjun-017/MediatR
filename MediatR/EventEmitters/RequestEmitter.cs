using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;
using MediatR.EventEmitters;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Publishers;

internal class RequestEmitter : BaseEmitter, ISender
{
    private readonly IQueryHandlerResolver _queryHandlerResolver;

    public RequestEmitter(IQueryHandlerResolver queryHandlerResolver, IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
        _queryHandlerResolver = queryHandlerResolver;
    }

    public async Task SendAsync(IRequest query, CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return;

        var queryType = query.GetType();
        var handlerType = _queryHandlerResolver.GetQueryHandlerType(queryType);

        var handler = GetHandlerInstance(handlerType);

        var handlerDelegate = _queryHandlerResolver.GetQueryHandlerExecutionDelegate(handlerType);
        await handlerDelegate(handler, query, token);
    }

    public async Task<T> SendAsync<T>(IRequest<T> query, CancellationToken token)
    {
        if (token.IsCancellationRequested) return await Task.FromCanceled<T>(token);

        var queryType = query.GetType();
        var handlerType = _queryHandlerResolver.GetQueryHandlerType(queryType);

        var handler = GetHandlerInstance(handlerType);

        var handlerDelegate = _queryHandlerResolver.GetQueryHandlerExecutionDelegateWithResult<T>(handlerType);
        T result = (T)await handlerDelegate(handler, query, token);

        return result;
    }

}
