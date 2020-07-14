using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MCMS.Emailing.Sender
{
    public interface IMEmailSender : IEmailSender
    {
        Task SendEmailAsync(string senderEmail, string senderName, string recipientAddress, string subject, string message);
    }
}