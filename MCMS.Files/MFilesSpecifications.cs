using System;
using System.IO;
using MCMS.Base.Builder;
using MCMS.Base.Data;
using MCMS.Base.Files.UploadPurpose;
using MCMS.Files.Filters;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace MCMS.Files
{
    public class MFilesSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<UploadPurposeOptions>();
            services.AddTransient<FileUploadManager>();
            services.ConfigureSwaggerGen(options => { options.SchemaFilter<SwaggerFilePurposesFilter>(); });
            services.AddScoped<IRepository<FileEntity>, FilesRepository>();
            services.AddScoped<FilesRepository>();
            services.AddScoped<FilesService>();
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            CreateAndRegisterDirectories(app, serviceProvider);
        }

        private void CreateAndRegisterDirectories(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<MFilesSpecifications>>();
            var neededDirs = new[] {MFiles.PublicPath, MFiles.PrivatePath};
            foreach (var neededDir in neededDirs)
            {
                if (!Directory.Exists(neededDir))
                {
                    logger.LogInformation("Creating directory '" + neededDir + "'.");
                    Directory.CreateDirectory(neededDir);
                }
            }

            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), MFiles.PublicPath));

            app.UseStaticFiles(new StaticFileOptions
                {FileProvider = new PhysicalFileProvider(path), RequestPath = MFiles.PublicVirtualPath});
        }
    }
}