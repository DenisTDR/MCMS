using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Builder
{
    public abstract class MSpecifications
    {
        public bool HasStaticFiles { get; set; }
        public string PrePublishRootPath { get; set; }
        public bool HasRazorViews { get; set; }
        public IWebHostEnvironment Environment { get; set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
        }

        public virtual void ConfigMvc(MvcOptions options)
        {
        }

        public virtual IMvcBuilder MvcChain(IMvcBuilder source)
        {
            return source;
        }

        public string GetAssemblyName()
        {
            return GetType().Assembly.GetName().Name;
        }
    }
}