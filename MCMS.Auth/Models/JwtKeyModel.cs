using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MCMS.Auth.Models
{
    public class JwtKeyModel
    {
        public string Key { get; set; }
        public DateTime Created { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static JwtKeyModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<JwtKeyModel>(json);
        }

        public SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}