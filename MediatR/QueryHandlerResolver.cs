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
        private ConcurrentDictionary<Type, Type> QueryHandlerMapping = new ConcurrentDictionary<Type, Type>();
        private ConcurrentDictionary<Type, QueryHandlerExecutionDelegate> QueryHandlerExecutionDelegateMapping = new ConcurrentDictionary<Type, QueryHandlerExecutionDelegate>();

        public void MapQueryToHandler(Assembly executingAssembly)
        {
            var assemblyTypes = executingAssembly.GetTypes().ToList();

            var queryTypes = assemblyTypes.Where(x => x.IsClass && !x.IsAbstract && typeof(IQuery).IsAssignableFrom(x));

            foreach (var queryType in queryTypes)
            {   
                var handlerTypes = assemblyTypes.Where(x => typeof(IQueryHandler<>).MakeGenericType(queryType).IsAssignableFrom(x)).ToList();

                if (handlerTypes.Count == 0) throw new InvalidOperationException($"No handler found for the query type {queryType.Name}");

                if (handlerTypes.Count > 1) throw new InvalidOperationException($"More than one handlers found for the type {queryType.Name}");

                var handlerType = handlerTypes.First();
                var handlerDelegate = ConvertMethodInfoToHandlerExecutionDelegate(handlerType);
                    
                QueryHandlerMapping.TryAdd(queryType, handlerType);
                QueryHandlerExecutionDelegateMapping.TryAdd(handlerType, handlerDelegate);
            }
        }

        public Type GetQueryHandlerType(Type query)
        {
            return QueryHandlerMapping[query];
        }

        public QueryHandlerExecutionDelegate GetQueryHandlerExecutionDelegate(Type queryHandler)
        {
            return QueryHandlerExecutionDelegateMapping[queryHandler];
        }

        public delegate Task QueryHandlerExecutionDelegate(object handlerInstance, object query, CancellationToken token);

        #region

        private QueryHandlerExecutionDelegate ConvertMethodInfoToHandlerExecutionDelegate(Type queryHandlerType)
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
            var delegateExpression = Expression.Lambda<QueryHandlerExecutionDelegate>(expressionCall, handlerInstanceParam, queryInstanceParam, tokenInstanceParam);
            
            return delegateExpression.Compile();
        }

        #endregion
    }
}
