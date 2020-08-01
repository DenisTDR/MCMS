using System.Linq;
using System.Security.Claims;

namespace MCMS.Base.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasRole(this ClaimsPrincipal claimsPrincipal, string role)
        {
            return claimsPrincipal.FindAll(ClaimTypes.Role).Any(claim => claim.Value == role);
        }
    }
}