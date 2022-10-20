using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MCMS.Auth.Identity
{
    internal static class IdentityServiceCollectionExtensions
    {
        public static IdentityBuilder AddDefaultIdentityWithBs4<TUser>(this IServiceCollection services,
            Action<IdentityOptions> configureOptions) where TUser : class
        {
            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = IdentityConstants.ApplicationScheme;
                    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies(o => { });

            return services.AddIdentityCore<TUser>(o =>
                {
                    o.Stores.MaxLengthForKeys = 128;
                    configureOptions?.Invoke(o);
                })
                .AddBs4Ui()
                .AddDefaultTokenProviders();
        }

        private static IdentityBuilder AddBs4Ui(this IdentityBuilder builder)
        {
            builder.AddSignInManager();
            builder.Services
                .AddMvc()
                .ConfigureApplicationPartManager(apm =>
                {
                    var parts = new ConsolidatedAssemblyApplicationPartFactory().GetApplicationParts(typeof(IdentityBuilderUIExtensions).Assembly);
                    foreach (var part in parts)
                    {
                        apm.ApplicationParts.Add(part);
                    }
                    apm.FeatureProviders.Add(new ViewVersionFeatureProvider());
                });

            builder.Services.ConfigureOptions(
                typeof(IdentityDefaultUiConfigureOptions<>)
                    .MakeGenericType(builder.UserType));
            builder.Services.TryAddTransient<IEmailSender, MockEmailSender>();

            return builder;
        }

        private class ViewVersionFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
        {

            public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
            {
                var viewsToRemove = new List<CompiledViewDescriptor>();
                foreach (var descriptor in feature.ViewDescriptors)
                {
                    if (IsIdentityUiView(descriptor))
                    {
                        if (descriptor.Type!.FullName!.Contains("V5"))
                        {
                            // Remove V5 views
                            viewsToRemove.Add(descriptor);
                        }
                        else
                        {
                            // Fix up paths to eliminate version subdir
                            descriptor.RelativePath = descriptor.RelativePath.Replace("V4/", "");
                        }
                    }
                }

                foreach (var descriptorToRemove in viewsToRemove)
                {
                    feature.ViewDescriptors.Remove(descriptorToRemove);
                }
            }

            private static bool IsIdentityUiView(CompiledViewDescriptor desc) => desc.RelativePath.StartsWith("/Areas/Identity", StringComparison.OrdinalIgnoreCase) && desc.Type.Assembly == typeof(IdentityBuilderUIExtensions).Assembly;
        }
    }
}