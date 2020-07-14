using MCMS.Base.Builder;
using MCMS.Base.Repositories;
using MCMS.Common.Translations.Languages;
using MCMS.Common.Translations.Translations;
using MCMS.Data;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Common
{
    public class MCommonSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<TranslationEntity>, TranslationsRepository>();
            services.AddScoped<TranslationsRepository>();
            services.AddScoped<IRepository<LanguageEntity>, LanguagesRepository>();
            services.AddScoped<LanguagesRepository>();
            services.AddScoped<ITranslationsRepository, TranslationsRepository>();
        }
    }
}