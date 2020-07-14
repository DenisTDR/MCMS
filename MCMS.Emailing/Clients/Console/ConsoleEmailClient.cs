using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MCMS.Emailing.Clients.Console
{
    public class ConsoleEmailClient : IMEmailClient
    {
        private ILogger<ConsoleEmailClient> _logger;

        public ConsoleEmailClient(ILogger<ConsoleEmailClient> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string senderEmail, string senderName, string recipientAddress, string subject,
            string message)
        {
            _logger.LogWarning($"Sending email:" +
                               $"\n To: {recipientAddress}" +
                               $"\n From: <{senderEmail}> {senderName}" +
                               $"\n Subject: {subject}" +
                               $"\n Content: {message}"
            );
            return Task.CompletedTask;
        }
    }
}