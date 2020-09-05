using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations.Item
{
    public class TranslationItemFormModel : IFormModel
    {
        // [FormlyFieldClassName("d-flex col-12 col-sm-3")]
        [FormlySelect(typeof(LanguagesAdminApiController), ClassName = "d-flex col-12 col-sm-3")]
        public virtual LanguageViewModel Language { get; set; }

        // [FormlyFieldClassName("d-flex col-12 col-sm-9")]
        [FormlyField(ClassName = "d-flex col-12 col-sm-9")]
        [DataType(DataType.MultilineText)]
        public string Value { get; set; }

        // public bool IsRichText { get; set; }
    }
}