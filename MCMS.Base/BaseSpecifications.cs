using System.Linq;
using System.Reflection;
using MCMS.Base.Attributes;
using MCMS.Base.Builder;
using MCMS.Base.Data.Seeder;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base
{
    public class BaseSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddOptions<SeedSources>()
                .Configure(ss => ss.Add((typeof(BaseSpecifications).Assembly, "seed-base.json")));
            RegisterServicesByAttribute(services);
        }

        private void RegisterServicesByAttribute(IServiceCollection services)
        {
            var serviceTypes = App.Specifications
                .Select(spec => spec.GetType().Assembly).Distinct()
                .SelectMany(ass => ass.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .Where(type => type.GetCustomAttribute<ServiceAttribute>() is { });

            foreach (var serviceType in serviceTypes)
            {
                var attr = serviceType.GetCustomAttribute<ServiceAttribute>();
                if (attr == null) continue;
                services.Add(new ServiceDescriptor(serviceType, serviceType, attr.Lifetime));
            }
        }
    }
}