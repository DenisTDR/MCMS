using System;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Attributes
{
    public class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; } = ServiceLifetime.Scoped;
        public Type ServiceType { get; }
        public Type OverrideImplementationFor { get; set; }
        public bool HasServiceType => ServiceType != null;

        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped, Type serviceType = null)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
        }

        public ServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }
}