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

            var handlerType = _queryHandlerResolver.GetQueryHandlerType(query.GetType());
            var handler = Activator.CreateInstance(handlerType);

            var handlerDelegate = _queryHandlerResolver.GetQueryHandlerExecutionDelegate(handlerType);

            await handlerDelegate(handler, query, token);
        }

        public Task<T> SendAsync<T>(IQuery<T> query, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
