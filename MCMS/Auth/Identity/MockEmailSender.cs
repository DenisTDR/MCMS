using System.Threading.Tasks;
using MCMS.Base.Exceptions;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MCMS.Auth.Identity
{
    internal class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new KnownException("There is no valid EmailSender provided");
        }
    }
}