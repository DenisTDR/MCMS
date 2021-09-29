using System.ComponentModel.DataAnnotations;

namespace MCMS.Auth.Tokens.Dtos
{
    public class TokenRequestDto
    {
        [Required]
        public string Token { get; set; }
    }
}