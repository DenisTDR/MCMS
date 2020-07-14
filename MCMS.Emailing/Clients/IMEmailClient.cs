using System.Threading.Tasks;

namespace MCMS.Emailing.Clients
{
    public interface IMEmailClient
    {
        Task SendEmailAsync(string senderEmail, string senderName, string recipientAddress, string subject, string message);
    }
}