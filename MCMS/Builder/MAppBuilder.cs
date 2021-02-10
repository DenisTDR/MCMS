using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base;
using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using MCMS.Base.SwaggerFormly.Models;
using MCMS.Data;
using MCMS.SwaggerFormly;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

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

        public MAppBuilder AddSpecifications(MSpecifications specifications, int? index = null)
        {
            specifications.Environment = _hostEnvironment;
            if (index != null)
            {
                _specifications.Insert(index.Value, specifications);
            }
            else
            {
                _specifications.Add(specifications);
            }

            return this;
        }

        public MAppBuilder AddSpecifications<T>(int? index = null) where T : MSpecifications, new()
        {
            AddSpecifications(new T(), index);
            return this;
        }

        private Action<IServiceCollection> _addDbContextAction;

        public MAppBuilder WithPostgres<T>(Action<NpgsqlDbContextOptionsBuilder> pgOptionsBuilder = null)
            where T : BaseDbContext
        {
            _addDbContextAction = services =>
            {
                services.AddDbContext<T>(optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(Env.GetOrThrow("DB_URL"),
                        o => { pgOptionsBuilder?.Invoke(o); });
                });

                // this is required because service provider must use the same instance for T and for BaseDbContext 
                // otherwise there will be two scoped instances and we don't like that
                services.AddScoped(serviceProvider => serviceProvider.GetRequiredService<BaseDbContext>() as T);
            };
            return this;
        }

        public MAppBuilder WithSwagger(params SwaggerConfigOptions[] configOptions)
        {
            return AddSpecifications(new SwaggerSpecifications(configOptions));
        }

        public MApp Build()
        {
            if (!_specifications.Any(spec => spec is BaseSpecifications))
            {
                AddSpecifications<BaseSpecifications>(0);
            }

            if (!_specifications.Any(spec => spec is MAuthSpecifications))
            {
                AddSpecifications<MAuthSpecifications>(0);
            }

            if (!_specifications.Any(spec => spec is MBaseSpecifications))
            {
                AddSpecifications<MBaseSpecifications>(0);
            }

            var app = new MApp(_hostEnvironment, _specifications, _addDbContextAction);
            _addDbContextAction = null;
            return app;
        }
    }
}