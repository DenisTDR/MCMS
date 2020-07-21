using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MCMS.Emailing.Clients.SendGrid
{
    public class SendGridEmailClient : IMEmailClient
    {
        private readonly ILogger<SendGridEmailClient> _logger;
        private readonly SendgridClientOptions _clientOptions;

        public SendGridEmailClient(ILogger<SendGridEmailClient> logger, IOptions<SendgridClientOptions> clientOptions)
        {
            _logger = logger;
            _clientOptions = clientOptions.Value;
        }

        public async Task SendEmailAsync(string senderEmail, string senderName, string recipientAddress, string subject,
            string message)
        {
            _logger.LogWarning($"Sending email with SendGrid: '{subject}' to '{recipientAddress}'");
            var client = new SendGridClient(_clientOptions.Key);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(senderEmail ?? _clientOptions.DefaultSenderAddress, senderName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(recipientAddress));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            var respStr = response.DeserializeResponseBodyAsync(response.Body);
        }
    }
}