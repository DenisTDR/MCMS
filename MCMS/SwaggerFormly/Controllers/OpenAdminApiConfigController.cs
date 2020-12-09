using System;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using Microsoft.AspNetCore.Mvc;
using MCMS.Controllers.Api;
using MCMS.SwaggerFormly.Translations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.SwaggerFormly.Controllers
{
    public class OpenApiConfigController : AdminApiController
    {
        private readonly SwaggerConfigService _swaggerConfigService;
        private readonly ITranslationsRepository _translationsRepository;

        public OpenApiConfigController(
            SwaggerConfigService swaggerConfigService,
            ITranslationsRepository translationsRepository
        )
        {
            _swaggerConfigService = swaggerConfigService;
            _translationsRepository = translationsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string name = "admin-api",
            [FromQuery] string lang = null,
            [FromHeader(Name = "X-LANG")] string headerLang = null)
        {
            try
            {
                var config = _swaggerConfigService.GetConfig(name);
                await EnsureTranslationsForLanguage(config, lang ?? headerLang);
                return Ok(config);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDelayed([FromQuery] string lang = null,
            [FromHeader(Name = "X-LANG")] string headerLang = null)
        {
            await Task.Delay(2000);
            return await Get(lang, headerLang);
        }

        private async Task EnsureTranslationsForLanguage(JObject config, string lang = null)
        {
            if (string.IsNullOrEmpty(lang))
            {
                lang = _translationsRepository.Language;
            }

            var changed = false;
            var transObj = config.ContainsKey("translations")
                ? config["translations"].ToObject<FormlyTranslations>()
                : new FormlyTranslations {DefaultLanguage = _translationsRepository.Language};

            if (!transObj.HasTranslationsFor(lang))
            {
                changed = true;
                transObj.AddTranslationsFor(lang, await _translationsRepository.GetAll(lang, "formly"));
            }

            if (changed)
            {
                config["translations"] =
                    JToken.FromObject(transObj, JsonSerializer.Create(Utils.DefaultJsonSerializerSettings()));
            }
        }
    }
}