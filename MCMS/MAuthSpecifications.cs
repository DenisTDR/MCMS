using System;
using MCMS.Admin.Users;
using MCMS.Auth;
using MCMS.Base.Auth;
using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.Data;
using MCMS.Data.Seeder;

namespace MCMS
{
    public class MAuthSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, Role>();
            
            services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = Env.GetBool("REQUIRE_CONFIRMED_ACCOUNT");
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<Role>()
                .AddClaimsPrincipalFactory<MUserClaimsPrincipalFactory>()
                .AddEntityFrameworkStores<BaseDbContext>();
            services.AddScoped<UsersTableModelDisplayConfigService>();
            services.AddScoped<AuthService>();
            services.AddOptions<EntitySeeders>().Configure(seeders => { seeders.Add<RolesSeeder>(); });
        }


        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}