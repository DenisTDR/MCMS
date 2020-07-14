using System.Threading.Tasks;
using MCMS.Emailing.Clients;

namespace MCMS.Emailing.Sender
{
    public class MEmailSender : IMEmailSender
    {
        private IMEmailClient _emailClient;

        public MEmailSender(IMEmailClient emailClient)
        {
            _emailClient = emailClient;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return _emailClient.SendEmailAsync(null, null, email, subject, htmlMessage);
        }

        public Task SendEmailAsync(string senderEmail, string senderName, string recipientAddress, string subject,
            string message)
        {
            return _emailClient.SendEmailAsync(senderEmail, senderName, recipientAddress, subject, message);
        }
    }
}