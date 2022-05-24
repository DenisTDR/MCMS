using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly ILogger _logger;
        private readonly MSendgridClientOptions _clientOptions;

        private SendGridClient _client;
        private SendGridClient Client => _client ??= new SendGridClient(_clientOptions.Key);

        public MSendGridEmailClient(
            ILoggerFactory loggerFactory,
            IOptions<MSendgridClientOptions> clientOptions)
        {
            _clientOptions = clientOptions.Value;
            _logger = loggerFactory.CreateLogger("MailClient");
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

            MapAddressList(message, msg, out var to);
            MapAttachments(message, msg);

            _logger.LogInformation("Sending mail with SendGrid:\nTo: {To}\nSubject: {Subject}", to, msg.Subject);

            msg.SetClickTracking(false, false);
            var response = await Client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                return true;
            }

            var respStr = await response.DeserializeResponseBodyAsync(response.Body);
            _logger.LogError("Mail send with SendGrid failed");
            _logger.LogError("{Json}", JsonConvert.SerializeObject(respStr));
            return false;
        }

        private void MapAttachments(MimeMessage message, SendGridMessage msg)
        {
            foreach (var srcAttach in message.Attachments)
            {
                if (srcAttach is not MimePart mimePart) continue;

                var attachment = new Attachment
                {
                    Type = mimePart.ContentType.MimeType,
                    Filename = mimePart.FileName
                };
                using var ms = new MemoryStream();
                mimePart.Content.Stream.CopyTo(ms);
                var bytes = ms.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                attachment.Content = base64;
                msg.Attachments ??= new List<Attachment>();
                msg.Attachments.Add(attachment);
            }
        }

        private void MapAddressList(MimeMessage message, SendGridMessage msg, out string to)
        {
            to = null;
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

            foreach (var addr in message.Cc)
            {
                if (addr is MailboxAddress mAddr)
                {
                    msg.AddCc(mAddr.Address, mAddr.Name);
                }
                else
                    throw new Exception("message.Cc must contain only MailboxAddress objects");
            }

            foreach (var addr in message.Bcc)
            {
                if (addr is MailboxAddress mAddr)
                {
                    msg.AddBcc(mAddr.Address, mAddr.Name);
                }
                else
                    throw new Exception("message.Bcc must contain only MailboxAddress objects");
            }

            if (message.ReplyTo.FirstOrDefault() is MailboxAddress replyTo)
            {
                msg.ReplyTo = new EmailAddress(replyTo.Address, replyTo.Name);
            }
        }
    }
}