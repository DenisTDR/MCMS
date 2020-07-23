using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using MCMS.Base.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace MCMS.Areas.Identity.Pages.Account
{
    public class ActivateAccountModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ActivateAccountModel> _logger;

        public ActivateAccountModel(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ActivateAccountModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult OnGet(string code = null, string userId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest("Please logout before trying to activate other account.");
            }

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId))
            {
                return BadRequest("A code and userId must be supplied for account activation.");
            }

            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
                UserId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId)),
            };
            return Page();
        }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
            public string UserId { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Couldn't find the provided user. Maybe it was been deleted.");
                return Page();
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully activated an account using link provided by email.");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}