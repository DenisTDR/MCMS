namespace MCMS.Base.Builder
{
    public class SiteConfig
    {
        public string SiteName { get; set; }
        public string SiteCopyright { get; set; }
        public string SiteLogo { get; set; }
        public bool HideSiteNameFromNavbar { get; set; }
        public string FaviconPath { get; set; } = "/_content/MCMS/favicon.ico";
    }
}