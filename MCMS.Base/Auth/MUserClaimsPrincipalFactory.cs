using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MCMS.Base.Auth
{
    public class MUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        public MUserClaimsPrincipalFactory(
            UserManager<User> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await UserManager.GetRolesAsync(user);
            identity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            identity.RemoveClaim(identity.FindFirst(ClaimTypes.Name));

            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            return identity;
        }
    }
}