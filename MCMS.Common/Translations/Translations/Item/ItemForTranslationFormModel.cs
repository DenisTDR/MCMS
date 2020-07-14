using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations.Item
{
    public class ItemForTranslationFormModel : TranslationItemFormModel
    {
        // [FormlyFieldClassName("d-flex col-12 col-sm-3")]
        // [FormlySelect(typeof(LanguagesAdminApiController))]
        [FormlyFieldProp("disabled", true, "templateOptions")]
        public override LanguageViewModel Language { get; set; }
    }
}