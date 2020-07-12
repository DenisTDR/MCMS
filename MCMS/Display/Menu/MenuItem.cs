using System.Linq;
using System.Security.Claims;

namespace MCMS.Display.Menu
{
    public abstract class MenuItem
    {
        public int Index { get; set; }
    }

    public interface IMenuItem
    {
        public int Index { get; set; }
        public string IconClasses { get; set; }
        public string[] RequiredRoles { get; set; }
    }

    public static class MenuItemExtensions
    {
        public static T WithIconClasses<T>(this T item, string iconClasses) where T : IMenuItem
        {
            item.IconClasses = iconClasses;
            return item;
        }

        public static T RequiresRoles<T>(this T item, params string[] requiredRoles) where T : IMenuItem
        {
            item.RequiredRoles = requiredRoles;
            return item;
        }

        public static bool SatisfiedByUSer<T>(this T item, ClaimsPrincipal claims)where T : IMenuItem
        {
            if (item.RequiredRoles == null)
            {
                return true;
            }

            return item.RequiredRoles.Any(role => claims.HasClaim(ClaimTypes.Role, role));
        }
    }
}