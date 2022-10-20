using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Auth.Identity
{
    internal class IdentityDefaultUiConfigureOptions<TUser> :
        IPostConfigureOptions<RazorPagesOptions>,
        IConfigureNamedOptions<CookieAuthenticationOptions> where TUser : class
    {
        private const string IdentityUiDefaultAreaName = "Identity";

        public IdentityDefaultUiConfigureOptions(
            IWebHostEnvironment environment)        {
            Environment = environment;
        }

        public IWebHostEnvironment Environment { get; }

        public void PostConfigure(string name, RazorPagesOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.Conventions.AuthorizeAreaFolder(IdentityUiDefaultAreaName, "/Account/Manage");
            options.Conventions.AuthorizeAreaPage(IdentityUiDefaultAreaName, "/Account/Logout");
            var convention = new IdentityPageModelConvention<TUser>();
            options.Conventions.AddAreaFolderApplicationModelConvention(
                IdentityUiDefaultAreaName,
                "/",
                pam => convention.Apply(pam));
            options.Conventions.AddAreaFolderApplicationModelConvention(
                IdentityUiDefaultAreaName,
                "/Account/Manage",
                pam => pam.Filters.Add(new ExternalLoginsPageFilter<TUser>()));
        }

        public void Configure(CookieAuthenticationOptions options) {
            // Nothing to do here as Configure(string name, CookieAuthenticationOptions options) is hte one setting things up.
        }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.Equals(IdentityConstants.ApplicationScheme, name, StringComparison.Ordinal))
            {
                options.LoginPath = $"/{IdentityUiDefaultAreaName}/Account/Login";
                options.LogoutPath = $"/{IdentityUiDefaultAreaName}/Account/Logout";
                options.AccessDeniedPath = $"/{IdentityUiDefaultAreaName}/Account/AccessDenied";
            }
        }
    }
}