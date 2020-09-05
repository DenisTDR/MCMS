using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MCMS.Emailing.Clients.Stdout
{
    public class MStdoutEmailClient : IMEmailClient
    {
        private readonly ILogger<MStdoutEmailClient> _logger;

        public MStdoutEmailClient(ILogger<MStdoutEmailClient> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendEmail(MimeMessage message)
        {
            _logger.LogWarning($"   Sending email:\nTo: {message}");
            return Task.FromResult(true);
        }
    }
}