using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations.Item
{
    public class ItemForTranslationFormModel : TranslationItemFormModel
    {
        [FormlyFieldProp("disabled", true, "templateOptions")]
        public override LanguageViewModel Language { get; set; }
    }
}