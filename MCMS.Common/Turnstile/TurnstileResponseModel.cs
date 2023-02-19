namespace MCMS.Common.Turnstile
{
    public class TurnstileResponseModel
    {
        public bool Success { get; set; }
        public string[] ErrorCodes { get; set; }
        public string[] Messages { get; set; }
    }
}