using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Abstractions;

namespace MediatR
{
    internal class QueryHandlerResolver : IQueryHandlerResolver
    {
        private ConcurrentDictionary<Type, Type> _queryHandlerMapping = new ConcurrentDictionary<Type, Type>();
        private ConcurrentDictionary<Type, QueryHandlerExecutionDelegate> _queryHandlerExecutionDelegateMapping = new ConcurrentDictionary<Type, QueryHandlerExecutionDelegate>();
        private ConcurrentDictionary<Type, QueryHandlerExecutionDelegateWithResult> _queryHandlerExecutionDelegateWithResultMapping = new ConcurrentDictionary<Type, QueryHandlerExecutionDelegateWithResult>();

        public void MapQueryToHandler(Assembly executingAssembly)
        {
            var assemblyTypes = executingAssembly.GetTypes().ToList();

            var queryTypes = assemblyTypes.Where(x => x.IsClass && !x.IsAbstract 
                && (typeof(IQuery).IsAssignableFrom(x) || typeof(IQuery<>).IsAssignableFrom(x)));

            foreach (var queryType in queryTypes)
            {
                var handlerContractType = queryType.IsGenericType ? typeof(IQueryHandler<>).MakeGenericType(queryType, queryType.GetGenericArguments().First())
                    : typeof(IQueryHandler<>).MakeGenericType(queryType);
                var handlerTypes = assemblyTypes.Where(x => handlerContractType.IsAssignableFrom(x)).ToList();

                if (handlerTypes.Count == 0) throw new InvalidOperationException($"No handler found for the query type {queryType.Name}");

                if (handlerTypes.Count > 1) throw new InvalidOperationException($"More than one handlers found for the type {queryType.Name}");

                var handlerType = handlerTypes.First();
                _queryHandlerMapping.TryAdd(queryType, handlerType);

                if(queryType.IsGenericType)
                {
                    var handlerDelegate = ConvertMethodInfoToHandlerExecutionDelegate<QueryHandlerExecutionDelegate>(handlerType);
                    _queryHandlerExecutionDelegateMapping.TryAdd(handlerType, handlerDelegate);
                }
                else {
                    var handlerDelegate = ConvertMethodInfoToHandlerExecutionDelegate<QueryHandlerExecutionDelegateWithResult>(handlerType);
                    _queryHandlerExecutionDelegateWithResultMapping.TryAdd(handlerType, handlerDelegate);
                }
                    
            }
        }

        public Type GetQueryHandlerType(Type query)
        {
            return _queryHandlerMapping[query];
        }

        public QueryHandlerExecutionDelegate GetQueryHandlerExecutionDelegate(Type queryHandler)
        {
            return _queryHandlerExecutionDelegateMapping[queryHandler];
        }

        public QueryHandlerExecutionDelegateWithResult GetQueryHandlerExecutionDelegateWithResult(Type queryHandler)
        {
            return _queryHandlerExecutionDelegateWithResultMapping[queryHandler];
        }

        public delegate Task QueryHandlerExecutionDelegate(object handlerInstance, object query, CancellationToken token);

        public delegate Task<object> QueryHandlerExecutionDelegateWithResult(object handlerInstance, object query, CancellationToken token);

        #region

        private TDelegate ConvertMethodInfoToHandlerExecutionDelegate<TDelegate>(Type queryHandlerType)
        {
            // (object handlerInstance, object query) => handlerInstance.HandleAsync(queryInstance, token)

            var handlerInstanceParam = Expression.Parameter(typeof(object));
            var queryInstanceParam = Expression.Parameter(typeof(object));
            var tokenInstanceParam = Expression.Parameter(typeof(CancellationToken));
            var handleAsyncMethodInfo = queryHandlerType.GetMethod("HandleAsync");

            var castedHandler = Expression.Convert(handlerInstanceParam, queryHandlerType);
            var parameterType = handleAsyncMethodInfo.GetParameters()[0].ParameterType;
            var castedQuery = Expression.Convert(queryInstanceParam, parameterType);

            var expressionCall = Expression.Call(castedHandler, handleAsyncMethodInfo, castedQuery, tokenInstanceParam);
            var delegateExpression = Expression.Lambda<TDelegate>(expressionCall, handlerInstanceParam, queryInstanceParam, tokenInstanceParam);
            
            return delegateExpression.Compile();
        }

        #endregion
    }
}
