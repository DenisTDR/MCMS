using System.Threading.Tasks;
using MCMS.Emailing.Clients;
using MimeKit;

namespace MCMS.Emailing.Sender
{
    public class MEmailSender : IMEmailSender
    {
        private IMEmailClient _emailClient;

        public MEmailSender(IMEmailClient emailClient)
        {
            _emailClient = emailClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmail(email, subject, htmlMessage);
        }

        public Task<bool> SendEmail(string email, string subject, string htmlMessage)
        {
            return SendEmail(null, null, email, subject, htmlMessage);
        }

        public Task<bool> SendEmail(string senderEmail, string senderName, string recipientAddress, string subject,
            string message)
        {
            var mimeMessage = new MimeMessage {Subject = subject};
            mimeMessage.To.Add(new MailboxAddress("", recipientAddress));
            mimeMessage.Body = new TextPart("html") {Text = message};
            if (senderEmail != null)
            {
                mimeMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            }

            return SendEmail(mimeMessage);
        }

        public Task<bool> SendEmail(MimeMessage message)
        {
            return _emailClient.SendEmail(message);
        }
    }
}