namespace MCMS.Auth.Tokens.Models
{
    public class SessionDto
    {
        public UserProfileDto Profile { get; set; }
        public TokenDto AccessToken { get; set; }
        public TokenDto RefreshToken { get; set; }
    }
}