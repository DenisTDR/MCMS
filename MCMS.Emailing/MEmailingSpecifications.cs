﻿using System;
using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using MCMS.Emailing.Clients;
using MCMS.Emailing.Clients.Gmail;
using MCMS.Emailing.Clients.SendGrid;
using MCMS.Emailing.Clients.Smtp;
using MCMS.Emailing.Clients.Stdout;
using MCMS.Emailing.Sender;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Emailing
{
    public class MEmailingSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            if (Env.Get("SENDGRID_KEY") is { } sendgridKey)
            {
                Console.WriteLine("Loading SendGrid emailing...");
                // get default sender here to throw on app start if not set
                var defaultSenderAddress = Env.GetOrThrow("SENDGRID_DEFAULT_SENDER");
                services.AddScoped<IMEmailClient, MSendGridClient>();
                services.AddOptions<MSendGridClientOptions>().Configure(options =>
                {
                    options.Key = sendgridKey;
                    options.DefaultSenderAddress = defaultSenderAddress;
                    options.DefaultSenderName = Env.Get("SENDGRID_DEFAULT_SENDER_NAME");
                });
            }
            else if (Env.Get("GMAIL_CREDENTIALS_JSON_PATH") is { } gmailCredentialsJsonPath)
            {
                Console.WriteLine("Loading GMail emailing...");
                // get token_json_path here to throw on app start if not set
                var gmailTokenJsonPath = Env.GetOrThrow("GMAIL_TOKEN_JSON_PATH");
                services.AddScoped<IMEmailClient, MGmailClient>();
                services.AddOptions<MGmailClientOptions>().Configure(options =>
                {
                    options.GmailCredentialsJsonPath = gmailCredentialsJsonPath;
                    options.GmailTokenJsonPath = gmailTokenJsonPath;
                });
            }
            else if (Env.Get("SMTP_HOST") is { } smtpHost)
            {
                Console.WriteLine("Loading SMTP emailing...");
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
            else
            {
                services.AddScoped<IMEmailClient, MStdoutEmailClient>();
            }

            services.AddScoped<IMEmailSender, MEmailSender>();
            services.AddScoped<IEmailSender, MEmailSender>();
        }
    }
}