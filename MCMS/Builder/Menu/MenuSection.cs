using System.Collections.Generic;

namespace MCMS.Builder.Menu
{
    public class MenuSection
    {
        public string Name { get; set; }
        public List<MenuLink> Links = new List<MenuLink>();
    }
}