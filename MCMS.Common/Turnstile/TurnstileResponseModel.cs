using Newtonsoft.Json;

namespace MCMS.Common.Turnstile
{
    public class TurnstileResponseModel
    {
        public bool Success { get; set; }
        [JsonProperty("error-codes")] public string[] ErrorCodes { get; set; }
        public string[] Messages { get; set; }
    }
}