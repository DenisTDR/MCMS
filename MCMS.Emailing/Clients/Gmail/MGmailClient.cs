using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MCMS.Base.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MCMS.Emailing.Clients.Gmail
{
    public class MGmailClient : IMEmailClient
    {
        private readonly ILogger _logger;

        private readonly MGmailClientOptions _clientOptions;
        private readonly SiteConfig _siteConfig;

        public MGmailClient(
            ILoggerFactory loggerFactory,
            IOptions<MGmailClientOptions> options,
            IOptions<SiteConfig> siteConfig)
        {
            _clientOptions = options.Value;
            _siteConfig = siteConfig.Value;
            _logger = loggerFactory.CreateLogger("MailClient");
        }

        public async Task<bool> SendEmail(MimeMessage message)
        {
            if (!message.To.Any(a => a is MailboxAddress))
                throw new Exception("No `to` address provided!");

            string[] scopes = { GmailService.Scope.GmailSend };

            await using var stream = new FileStream(_clientOptions.GmailCredentialsJsonPath, FileMode.Open,
                FileAccess.Read);

            var credPath = _clientOptions.GmailTokenJsonPath;
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
                scopes, "user", CancellationToken.None, new FileDataStore(credPath, true));


            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _siteConfig.SiteName
            });

            if (message.From.Count == 0 && !string.IsNullOrEmpty(_clientOptions.DefaultSenderAddress))
            {
                message.From.Add(new MailboxAddress(_clientOptions.DefaultSenderName,
                    _clientOptions.DefaultSenderAddress));
            }

            var toStr = string.Join(", ", message.To.Where(a => a is MailboxAddress).Cast<MailboxAddress>()
                .Select(a => $"<{a.Address}> {a.Name}".Trim()));

            _logger.LogInformation("Sending mail with GMail:\nTo: {To}\nSubject: {Subject}", toStr,
                message.Subject);

            var ms = new MemoryStream();
            await message.WriteToAsync(ms);
            var bytes = ms.ToArray();
            var encodedEmail = Convert.ToBase64String(bytes);
            var gmailMessage = new Message { Raw = encodedEmail };

            var req = service.Users.Messages.Send(gmailMessage, "me");
            await req.ExecuteAsync();

            return true;
        }
    }
}