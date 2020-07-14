using System.Collections.Generic;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Common.Translations.Translations.Item;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationFormModel : IFormModel
    {
        [FormlyFieldClassName("d-flex col-12 col-sm-6 col-md-4")]
        public string Slug { get; set; }

        [FormlyFieldDefaultValue(false)] public bool IsRichText { get; set; }

        [FormlyFieldProp("fieldGroupClassName", "d-flex")]
        [FormlyFieldProp("addDisabled", true, "templateOptions")]
        [FormlyFieldProp("removeDisabled", true, "templateOptions")]
        public List<ItemForTranslationFormModel> Items { get; set; }
    }
}