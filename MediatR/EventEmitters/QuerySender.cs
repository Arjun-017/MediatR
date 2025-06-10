using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;

namespace MediatR.Publishers
{
    internal class QuerySender : ISender
    {
        private readonly IQueryHandlerResolver _queryHandlerResolver;
        public QuerySender(IQueryHandlerResolver queryHandlerResolver)
        {
            _queryHandlerResolver = queryHandlerResolver;
        }

        public async Task SendAsync(IQuery query, CancellationToken token = default)
        {
            if (token.IsCancellationRequested) return;

            var queryType = query.GetType();
            var handlerType = _queryHandlerResolver.GetQueryHandlerType(queryType);
            var handler = Activator.CreateInstance(handlerType);
            
            var handlerDelegate = _queryHandlerResolver.GetQueryHandlerExecutionDelegate(handlerType);
            await handlerDelegate(handler, query, token);
        }

        public async Task<T> SendAsync<T>(IQuery<T> query, CancellationToken token)
        {
            if (token.IsCancellationRequested) return await Task.FromCanceled<T>(token);

            var returnType = typeof(T);
            var queryType = query.GetType();
            var handlerType = _queryHandlerResolver.GetQueryHandlerType(queryType, returnType);
            var handler = Activator.CreateInstance(handlerType);

            var handlerDelegate = _queryHandlerResolver.GetQueryHandlerExecutionDelegateWithResult<T>(handlerType);
            T result = (T)await handlerDelegate(handler, query, token);

            return result;
        }
    }
}
