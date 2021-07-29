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

        public async Task<bool> SendEmail(MimeMessage mimeMessage)
        {
            string[] scopes = {GmailService.Scope.GmailSend};

            await using var stream = new FileStream(_clientOptions.GmailCredentialsJsonPath, FileMode.Open,
                FileAccess.Read);

            var credPath = _clientOptions.GmailTokenJsonPath;
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                scopes, "user", CancellationToken.None, new FileDataStore(credPath, true));


            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _siteConfig.SiteName
            });


            var toAddr = mimeMessage.To.FirstOrDefault(a => a is MailboxAddress) as MailboxAddress ??
                         throw new Exception("Invalid to address");

            var to = $"<{toAddr.Address}> {toAddr.Name}";

            _logger.LogInformation("Sending mail with Gmail:\nTo: {To}\nSubject: {Subject}", to, mimeMessage.Subject);

            var ms = new MemoryStream();
            await mimeMessage.WriteToAsync(ms);
            var bytes = ms.ToArray();
            var encodedEmail = Convert.ToBase64String(bytes);
            var message = new Message {Raw = encodedEmail};

            var req = service.Users.Messages.Send(message, "me");
            var x = await req.ExecuteAsync();

            return true;
        }
    }
}