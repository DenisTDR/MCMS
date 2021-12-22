using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Auth;
using MCMS.Base.Data.Entities;
using Newtonsoft.Json;

namespace MCMS.Auth.Tokens.Models
{
    [Table("AspNetUserRefreshTokens")]
    public class RefreshTokenEntity : Entity
    {
        [Required] [JsonIgnore] public User User { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string CreatedByIp { get; set; }
        [ConcurrencyCheck] public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public string ReasonRevoked { get; set; }

        public bool IsExpired => DateTime.Now >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}