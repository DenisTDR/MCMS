using System;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Attributes
{
    public class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Lifetime = lifetime;
        }
    }
}