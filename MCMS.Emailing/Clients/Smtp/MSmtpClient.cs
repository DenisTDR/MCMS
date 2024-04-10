using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MCMS.Emailing.Clients.Smtp
{
    public class MSmtpClient : IMEmailClient
    {
        private readonly ILogger _logger;
        private readonly MSmtpClientOptions _clientOptions;

        public MSmtpClient(
            IOptions<MSmtpClientOptions> clientOptions,
            ILoggerFactory loggerFactory
        )
        {
            _clientOptions = clientOptions.Value;
            _logger = loggerFactory.CreateLogger("MailClient");
        }

        public async Task<bool> SendEmail(MimeMessage message)
        {
            if (!message.To.Any(a => a is MailboxAddress))
                throw new Exception("No `to` address provided!");

            if (!message.From.Any())
            {
                message.From.Add(new MailboxAddress(_clientOptions.DefaultSenderName, _clientOptions.DefaultSender));
            }

            var toStr = string.Join(", ", message.To.Where(a => a is MailboxAddress).Cast<MailboxAddress>()
                .Select(a => $"<{a.Address}> {a.Name}".Trim()));
            _logger.LogInformation("Sending mail with SMTP:\nTo: {To}\nSubject: {Subject}", toStr,
                message.Subject);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_clientOptions.Host, _clientOptions.Port, true);
            await smtp.AuthenticateAsync(_clientOptions.Email, _clientOptions.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
            return true;
        }
    }
}