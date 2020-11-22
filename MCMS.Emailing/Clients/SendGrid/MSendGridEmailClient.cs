using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MCMS.Emailing.Clients.SendGrid
{
    public class MSendGridEmailClient : IMEmailClient
    {
        private readonly ILogger<MSendGridEmailClient> _logger;
        private readonly MSendgridClientOptions _clientOptions;

        private SendGridClient _client;
        private SendGridClient Client => _client ??= new SendGridClient(_clientOptions.Key);

        public MSendGridEmailClient(ILogger<MSendGridEmailClient> logger,
            IOptions<MSendgridClientOptions> clientOptions)
        {
            _logger = logger;
            _clientOptions = clientOptions.Value;
        }
        
        // TODO: convert attachments too (from MimeMessage to SendGridMessage)
        public async Task<bool> SendEmail(MimeMessage message)
        {
            var sender = message.From.Mailboxes.FirstOrDefault();

            var msg = new SendGridMessage
            {
                From = new EmailAddress(sender?.Address ?? _clientOptions.DefaultSenderAddress,
                    sender?.Name ?? _clientOptions.DefaultSenderName),
                Subject = message.Subject,
                PlainTextContent = message.TextBody,
                HtmlContent = message.HtmlBody
            };

            var to = "";

            foreach (var addr in message.To)
            {
                if (addr is MailboxAddress mAddr)
                {
                    to = $"<{mAddr.Address}> {mAddr.Name}";
                    msg.AddTo(mAddr.Address, mAddr.Name);
                }
                else
                    throw new Exception("message.To must contain only MailboxAddress objects");
            }

            if (message.ReplyTo.FirstOrDefault() is MailboxAddress replyTo)
            {
                msg.ReplyTo = new EmailAddress(replyTo.Address, replyTo.Name);
            }

            _logger.LogInformation("Sending mail with SendGrid:\nTo: " + to + "\nSubject: " + msg.Subject);

            msg.SetClickTracking(false, false);
            var response = await Client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                return true;
            }

            var respStr = await response.DeserializeResponseBodyAsync(response.Body);
            _logger.LogError("Mail send with SendGrid failed.");
            _logger.LogError(JsonConvert.SerializeObject(respStr));
            return false;
        }
    }
}