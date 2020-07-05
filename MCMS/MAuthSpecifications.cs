using System;
using MCMS.Base.Auth;
using MCMS.Base.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.Builder;
using MCMS.Data;

namespace MCMS
{
    public class MAuthSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<BaseDbContext>();
        }


        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}