using System;
using System.Linq;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MCMS.Base.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IRepository<T> GetRepo<T>(this IServiceProvider serviceProvider) where T : class, IEntity
        {
            return serviceProvider.GetRequiredService<IRepository<T>>();
        }

        public static IRepository<T> Repo<T>(this IServiceProvider serviceProvider) where T : class, IEntity
        {
            return serviceProvider.GetRepo<T>();
        }

        public static object GetRepo(this IServiceProvider serviceProvider, Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                throw new Exception($"Type '{entityType.FullName}' does not inherit '{nameof(IEntity)}'.");
            }

            var genericMethodInfo = typeof(ServiceProviderExtensions).GetMethods()
                .FirstOrDefault(mi => mi.Name == nameof(GetRepo) && mi.IsGenericMethod);
            var methodInfo = genericMethodInfo?.MakeGenericMethod(entityType);
            return methodInfo?.Invoke(null, new object[] { serviceProvider });
        }

        public static T GetOptions<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService<IOptions<T>>()?.Value;
        }

        public static T GetRequiredOptions<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetRequiredService<IOptions<T>>().Value;
        }
    }
}