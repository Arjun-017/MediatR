using System.Reflection;
using MediatR.Abstractions;
using MediatR.Publishers;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR
{
    public static class IOCExtension
    {
        public static IServiceCollection AddMediater(this IServiceCollection services, Assembly executingAssembly)
        {
            services.AddSingleton<IQueryHandlerResolver>((sp) =>
            {
                IQueryHandlerResolver resolver = new QueryHandlerResolver();
                resolver.AddAssemblyTypes(executingAssembly);
            
                return resolver;
            });

            services.AddSingleton<ISender, QuerySender>();

            return services;
        } 
    }
}
