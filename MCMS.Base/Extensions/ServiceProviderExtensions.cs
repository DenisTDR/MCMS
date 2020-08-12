using System;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IRepository<T> GetRepo<T>(this IServiceProvider serviceProvider) where T : class, IEntity
        {
            return serviceProvider.GetService<IRepository<T>>();
        }
    }
}