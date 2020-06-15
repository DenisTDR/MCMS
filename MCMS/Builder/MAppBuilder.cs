using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Data;
using MCMS.Helpers;
using MCMS.SwaggerFormly;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Builder
{
    public class MAppBuilder
    {
        private readonly IList<MSpecifications> _specifications = new List<MSpecifications>();

        private readonly IWebHostEnvironment _hostEnvironment;

        public MAppBuilder(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public MAppBuilder AddSpecifications(MSpecifications specifications)
        {
            _specifications.Add(specifications);
            return this;
        }

        public MAppBuilder AddSpecifications<T>() where T : MSpecifications, new()
        {
            AddSpecifications(new T());
            return this;
        }

        private Action<IServiceCollection> _addDbContextAction;

        public MAppBuilder WithPostgres<T>() where T : BaseDbContext
        {
            _addDbContextAction = services =>
            {
                services.AddDbContext<T>(optionsBuilder => { optionsBuilder.UseNpgsql(Env.GetOrThrow("DB_URL")); });
            };
            return this;
        }

        public MAppBuilder WithSwagger(SwaggerConfigOptions swaggerConfigOptions = null)
        {
            return AddSpecifications(new SwaggerSpecifications(swaggerConfigOptions));
        }

        public MApp Build()
        {
            if (!_specifications.Any(spec => spec is MAuthSpecifications))
            {
                AddSpecifications<MAuthSpecifications>();
            }

            if (!_specifications.Any(spec => spec is MBaseSpecifications))
            {
                AddSpecifications<MBaseSpecifications>();
            }

            var addDbContextAction = _addDbContextAction;
            _addDbContextAction = null;
            return new MApp(_hostEnvironment, _specifications, addDbContextAction);
        }
    }
}