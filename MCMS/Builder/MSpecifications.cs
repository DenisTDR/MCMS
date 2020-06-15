using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Builder
{
    public abstract class MSpecifications
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }
        public virtual void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
        }
    }
}