using MCMS.Base.Auth.Interceptors;
using MCMS.Base.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Common.Turnstile
{
    public class TurnstileSpecifications : MSpecifications
    {
        public TurnstileConfig Config { get; } = new();

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<LayoutIncludesOptions>().Configure(c =>
            {
                c.AddForPages("Turnstile/TurnstileIncludes.cshtml");
            });

            services.AddScoped<IMAuthInterceptor, TurnstileAuthInterceptor>();
            services.AddSingleton<TurnstileValidationService>();

            services.AddOptions<TurnstileConfig>().Configure(config =>
            {
                config.IsEnabled = Config.IsEnabled;
                config.SiteKey = Config.SiteKey;
                config.SecretKey = Config.SecretKey;
                config.IncludeFormPaths = Config.IncludeFormPaths;
            });
        }
    }
}