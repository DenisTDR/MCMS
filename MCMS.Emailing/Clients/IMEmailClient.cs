using System.Threading.Tasks;
using MimeKit;

namespace MCMS.Emailing.Clients
{
    public interface IMEmailClient
    {
        Task<bool> SendEmail(MimeMessage message);
    }
}