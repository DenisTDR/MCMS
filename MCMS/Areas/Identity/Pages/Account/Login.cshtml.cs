using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Auth;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MCMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEnumerable<IMAuthInterceptor> _authInterceptors;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<User> signInManager,
            ILogger<LoginModel> logger,
            UserManager<User> userManager,
            IEnumerable<IMAuthInterceptor> authInterceptors)
        {
            _userManager = userManager;
            _authInterceptors = authInterceptors;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required] [EmailAddress] public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, false);
                if (result.Succeeded && _authInterceptors.Any())
                {
                    foreach (var mAuthInterceptor in _authInterceptors)
                    {
                        result = await mAuthInterceptor.OnSignIn(user, result, SignInType.Dashboard);
                    }
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "You are not allowed to sign in here.");
                    return Page();
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe,
                    lockoutOnFailure: false);


                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in");
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new {ReturnUrl = returnUrl, RememberMe = Input.RememberMe});
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    if (Env.GetBool("SHOW_NON_CONFIRMED_ACCOUNT_ALERT"))
                    {
                        // var user = await _userManager.FindByEmailAsync(Input.Email);
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError(string.Empty,
                                "The email address is not confirmed. Please check your inbox for the activation email. If you checked the spam folder and still not found one please contact us.");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}