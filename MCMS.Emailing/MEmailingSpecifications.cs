using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using MCMS.Emailing.Clients;
using MCMS.Emailing.Clients.Console;
using MCMS.Emailing.Clients.SendGrid;
using MCMS.Emailing.Sender;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Emailing
{
    public class MEmailingSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            if (Env.Get("SENDGRID_KEY") is {} sendgridKey)
            {
                // get default sender here to throw on app start
                var defaultSenderAddress = Env.GetOrThrow("SENDGRID_DEFAULT_SENDER");
                services.AddScoped<IMEmailClient, SendGridEmailClient>();
                services.AddOptions<SendgridClientOptions>().Configure(options =>
                {
                    options.Key = sendgridKey;
                    options.DefaultSenderAddress = defaultSenderAddress;
                    options.DefaultSenderName = Env.Get("SENDGRID_DEFAULT_SENDER_NAME");
                });
            }
            else
            {
                services.AddScoped<IMEmailClient, ConsoleEmailClient>();
            }

            services.AddScoped<IMEmailSender, MEmailSender>();
            services.AddScoped<IEmailSender, MEmailSender>();
        }
    }
}