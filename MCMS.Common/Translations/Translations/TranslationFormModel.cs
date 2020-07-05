using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationFormModel : IFormModel
    {
        public string Slug { get; set; }
        [DataType(DataType.MultilineText)] public string Value { get; set; }
        public bool IsRichText { get; set; }

        [FormlySelect(typeof(LanguagesAdminApiController))]
        public LanguageViewModel Language { get; set; }
    }
}