using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Remove<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);

            return services;
        }
    }
}