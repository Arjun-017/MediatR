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
        private ConcurrentBag<Type> _queryHandlerTypes;
        private ConcurrentDictionary<Type, Type> _queryHandlerMapping = new ConcurrentDictionary<Type, Type>();
        private ConcurrentDictionary<Type, QueryHandlerExecutionDelegate> _queryHandlerExecutionDelegateMapping = new ConcurrentDictionary<Type, QueryHandlerExecutionDelegate>();
        private ConcurrentDictionary<Type, Delegate> _queryHandlerExecutionDelegateWithResultMapping = new ConcurrentDictionary<Type, Delegate>();
        // keeping it delegate coz concurrent dictionary does not support generic type

        public void AddAssemblyTypes(Assembly executingAssembly)
        {
            var assemblyTypes = executingAssembly.GetTypes().ToList();
            var queryHandlerTypes = assemblyTypes.Where(x => x.IsClass && !x.IsAbstract && typeof(IBaseHandler).IsAssignableFrom(x));
            _queryHandlerTypes = new ConcurrentBag<Type>(queryHandlerTypes);
        }

        public Type GetQueryHandlerType(Type queryType, Type returnType = default)
        {
            return _queryHandlerMapping.GetOrAdd(queryType, (key) =>
            {
                var handlerContractType = returnType != default ? typeof(IQueryHandler<,>).MakeGenericType(new Type[] { key, returnType })
                : typeof(IQueryHandler<>).MakeGenericType(key);

                var handlerTypes = _queryHandlerTypes.Where(x => handlerContractType.IsAssignableFrom(x)).ToList();

                if (handlerTypes.Count == 0) throw new InvalidOperationException($"No handler found for the query type {key.Name}");

                if (handlerTypes.Count > 1) throw new InvalidOperationException($"More than one handlers found for the type {key.Name}");

                var handlerType = handlerTypes.First();
                
                return handlerType;
            });
        }

        public QueryHandlerExecutionDelegate GetQueryHandlerExecutionDelegate(Type queryHandler)
        {
            return _queryHandlerExecutionDelegateMapping.GetOrAdd(queryHandler, (key) =>
            {
                return ConvertMethodInfoToHandlerExecutionDelegate<QueryHandlerExecutionDelegate>(key);
            });
        }
         
        public QueryHandlerExecutionDelegateWithResult<TResult> GetQueryHandlerExecutionDelegateWithResult<TResult>(Type queryHandler)
        {
            return (QueryHandlerExecutionDelegateWithResult<TResult>)_queryHandlerExecutionDelegateWithResultMapping.GetOrAdd(queryHandler, (key) =>
            {
                return ConvertMethodInfoToHandlerExecutionDelegate<QueryHandlerExecutionDelegateWithResult<TResult>>(key);
            });
        }

        public delegate Task QueryHandlerExecutionDelegate(object handlerInstance, object query, CancellationToken token);

        public delegate Task<TResult> QueryHandlerExecutionDelegateWithResult<TResult>(object handlerInstance, object query, CancellationToken token);

        #region

        private TDelegate ConvertMethodInfoToHandlerExecutionDelegate<TDelegate>(Type queryHandlerType) where TDelegate : Delegate
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
            
            var delegateExpression = Expression.Lambda(typeof(TDelegate), expressionCall, handlerInstanceParam, queryInstanceParam, tokenInstanceParam);
            
            return (TDelegate)delegateExpression.Compile();
        }

        #endregion
    }
}
