using System;
using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using MCMS.Emailing.Clients;
using MCMS.Emailing.Clients.Brevo;
using MCMS.Emailing.Clients.Gmail;
using MCMS.Emailing.Clients.SendGrid;
using MCMS.Emailing.Clients.Smtp;
using MCMS.Emailing.Clients.Stdout;
using MCMS.Emailing.Sender;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Emailing;

public class MEmailingSpecifications : MSpecifications
{
    public override void ConfigureServices(IServiceCollection services)
    {
        if (Env.Get("SENDGRID_KEY") is { } sendgridKey && sendgridKey != string.Empty)
        {
            Console.WriteLine("Loading SendGrid emailing...");
            // get default sender here to throw on app start error if not set
            var defaultSenderAddress = Env.GetOrThrow("SENDGRID_DEFAULT_SENDER");
            services.AddScoped<IMEmailClient, MSendGridClient>();
            services.AddOptions<MSendGridClientOptions>().Configure(options =>
            {
                options.Key = sendgridKey;
                options.DefaultSenderAddress = defaultSenderAddress;
                options.DefaultSenderName = Env.Get("SENDGRID_DEFAULT_SENDER_NAME");
            });
        }
        else if (Env.Get("GMAIL_CREDENTIALS_JSON_PATH") is { } gmailCredentialsJsonPath &&
                 gmailCredentialsJsonPath != string.Empty)
        {
            Console.WriteLine("Loading GMail emailing...");
            // get token_json_path here to throw on app start if not set
            var gmailTokenJsonPath = Env.GetOrThrow("GMAIL_TOKEN_JSON_PATH");
            services.AddScoped<IMEmailClient, MGmailClient>();
            services.AddOptions<MGmailClientOptions>().Configure(options =>
            {
                options.GmailCredentialsJsonPath = gmailCredentialsJsonPath;
                options.GmailTokenJsonPath = gmailTokenJsonPath;
                options.DefaultSenderName = Env.Get("GMAIL_DEFAULT_SENDER_NAME");
                options.DefaultSenderAddress = Env.Get("GMAIL_DEFAULT_SENDER");
            });
        }
        else if (Env.Get("SMTP_HOST") is { } smtpHost && smtpHost != string.Empty)
        {
            Console.WriteLine("Loading SMTP emailing...<{0}>", smtpHost);
            var port = int.Parse(Env.GetOrThrow("SMTP_PORT"));
            var email = Env.GetOrThrow("SMTP_EMAIL");
            var password = Env.GetOrThrow("SMTP_PASSWORD");
            var defaultSender = Env.GetOrThrow("SMTP_DEFAULT_SENDER");
            var defaultSenderName = Env.GetOrThrow("SMTP_DEFAULT_SENDER_NAME");

            services.AddScoped<IMEmailClient, MSmtpClient>();
            services.AddOptions<MSmtpClientOptions>().Configure(options =>
            {
                options.Host = smtpHost;
                options.Port = port;
                options.Email = email;
                options.Password = password;
                options.DefaultSender = defaultSender;
                options.DefaultSenderName = defaultSenderName;
            });
        }
        else if (Env.Get("BREVO_API_KEY") is { } brevoApiKey && !string.IsNullOrEmpty(brevoApiKey))
        {
            Console.WriteLine("Loading Brevo emailing...");
            var senderAddress = Env.GetOrThrow("BREVO_SENDER_ADDRESS");

            services.AddScoped<IMEmailClient, MBrevoClient>();
            services.AddOptions<MBrevoClientOptions>().Configure(options =>
            {
                options.ApiKey = brevoApiKey;
                options.DefaultSenderAddress = senderAddress;
                options.DefaultSenderName = Env.Get("BREVO_SENDER_NAME");
            });
        }
        else
        {
            services.AddSingleton<IMEmailClient, MStdoutEmailClient>();
        }

        services.AddScoped<IMEmailSender, MEmailSender>();
        services.AddScoped<IEmailSender, MEmailSender>();
    }
}