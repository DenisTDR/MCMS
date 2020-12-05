using System;
using System.IO;
using System.Threading.Tasks;
using MCMS.Auth.Jwt;
using MCMS.Auth.Models;
using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MCMS.Auth
{
    public class MJwtAuthSpecifications : MSpecifications
    {
        private string _keyDir;

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IJwtFactory, JwtFactory>();
            ConfigureJwtOptions(services);
        }

        private void ConfigureJwtOptions(IServiceCollection services)
        {
            EnsureKeysDir();
            var keyModel = GetOrCreateKey().Result;

            var audience = Env.GetOrThrow("EXTERNAL_URL");
            var issuer = Env.GetOrThrow("EXTERNAL_URL");
            var credentials = keyModel.GetSigningCredentials();

            services.Configure<JwtOptions>(options =>
            {
                options.Audience = audience;
                options.Issuer = issuer;
                options.SignInCredentials = credentials;
            });

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.Audience = audience;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = credentials.Key,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });
        }

        private async Task<JwtKeyModel> GetOrCreateKey()
        {
            var keyPath = Path.Combine(_keyDir, "key.json");
            JwtKeyModel model = null;
            if (File.Exists(keyPath))
            {
                await using var fs = new FileStream(keyPath, FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fs);
                try
                {
                    model = JwtKeyModel.FromJson(await sr.ReadToEndAsync());
                }
                catch
                {
                    //
                }
            }

            if (model == null)
            {
                model = new JwtKeyModel {Key = Utils.GenerateRandomHexString(60), Created = DateTime.Now};
                await using var fs = new FileStream(keyPath, FileMode.CreateNew, FileAccess.Write);
                await using var sw = new StreamWriter(fs);
                await sw.WriteLineAsync(model.ToJson());
            }

            return model;
        }

        private void EnsureKeysDir()
        {
            if (Environment.IsProduction() && string.IsNullOrEmpty(Env.Get("PERSISTED_KEYS_DIRECTORY")))
            {
                Utils.DieWith("env var 'PERSISTED_KEYS_DIRECTORY' is required in Production");
            }

            _keyDir = Env.Get("PERSISTED_KEYS_DIRECTORY") ?? Path.Combine(Env.GetOrThrow("CONTENT_PATH"), "keys");
            if (!Directory.Exists(_keyDir))
            {
                Directory.CreateDirectory(_keyDir);
            }
        }
    }
}