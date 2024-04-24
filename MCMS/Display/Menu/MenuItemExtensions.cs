using System.Linq;
using System.Security.Claims;
using MCMS.Display.Link;

namespace MCMS.Display.Menu
{
    public static class MenuItemExtensions
    {
        public static T RequiresRoles<T>(this T item, params string[] requiredRoles) where T : IItemWithRequiredRoles
        {
            item.RequiredRoles = requiredRoles;
            return item;
        }

        public static bool SatisfiedByUser<T>(this T item, ClaimsPrincipal claims) where T : IItemWithRequiredRoles
        {
            if (item.RequiredRoles == null || !item.RequiredRoles.Any())
            {
                return true;
            }

            return item.RequiredRoles.Any(role => claims.HasClaim(ClaimTypes.Role, role));
        }
    }
}