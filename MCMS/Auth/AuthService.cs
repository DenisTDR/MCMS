using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MCMS.Base.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace MCMS.Auth
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthService(UserManager<User> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task SendActivationEmail(User user, IUrlHelper urlHelper, string scheme)
        {
            var subject = "Activate your account";
            var body =
                "Hello {{fullName}}, <br/><br/>Please activate your account by <a href='{{activationUrl}}'>clicking here</a>.";
            await SendActivationEmail(user, urlHelper, scheme, subject, body);
        }

        public async Task SendActivationEmail(User user, IUrlHelper urlHelper, string scheme, string subject,
            string body)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id));
            var callbackUrl = urlHelper.Page(
                "/Account/ActivateAccount",
                pageHandler: null,
                values: new {area = "Identity", code, userId},
                protocol: scheme);
            var encodedUrl = HtmlEncoder.Default.Encode(callbackUrl);

            body = body.Replace("{{fullName}}", user.FullName);
            body = body.Replace("{{activationUrl}}", encodedUrl);

            await _emailSender.SendEmailAsync(
                // user.Email.Split("@")[0] + "@tdrs.ro",
                user.Email, 
                subject, body);
        }
    }
}