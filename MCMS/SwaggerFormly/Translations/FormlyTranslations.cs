using System.Collections.Generic;

namespace MCMS.SwaggerFormly.Translations
{
    public class FormlyTranslations
    {
        public string DefaultLanguage { get; set; }
        public Dictionary<string, Dictionary<string, string>> Translations { get; set; }

        public bool HasTranslationsFor(string language)
        {
            return Translations?.ContainsKey(language) == true;
        }

        public void AddTranslationsFor(string language, Dictionary<string, string> translations)
        {
            Translations ??= new Dictionary<string, Dictionary<string, string>>();
            Translations[language] = translations;
        }
    }
}