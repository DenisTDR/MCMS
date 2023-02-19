namespace MCMS.Common.Turnstile
{
    public class TurnstileConfig
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
        public bool IsEnabled { get; set; }

        public string[] IncludeFormPaths { get; set; } =
            { "/Identity/Account/Login", "/Identity/Account/ForgotPassword" };
    }
}