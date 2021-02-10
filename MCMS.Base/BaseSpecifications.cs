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
        }
    }
}