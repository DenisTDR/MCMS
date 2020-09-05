using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace MCMS.Emailing.Sender
{
    public interface IMEmailSender: IEmailSender
    {
        Task<bool> SendEmail(string email, string subject, string htmlMessage);
        Task<bool> SendEmail(string senderEmail, string senderName, string recipientAddress, string subject, string message);
        Task<bool> SendEmail(MimeMessage message);
    }
}