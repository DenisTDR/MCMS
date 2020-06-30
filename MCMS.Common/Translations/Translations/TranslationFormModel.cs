using System.ComponentModel.DataAnnotations;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Common.Translations.Languages;
using MCMS.SwaggerFormly.Models;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationFormModel : IFormModel
    {
        public string Slug { get; set; }
        [DataType(DataType.MultilineText)] public string Value { get; set; }
        public bool IsRichText { get; set; }

        [FormlySelect("/api/LanguagesApi")]
        public LanguageViewModel Language { get; set; }
    }
}