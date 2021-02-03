using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Common.Translations.Translations.Item;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationFormModel : IFormModel
    {
        [FormlyField(ClassName = "d-flex col-12 col-sm-6 col-md-4")]
        [Required]
        public string Slug { get; set; }

        [FormlyField(ClassName = "d-flex col-12 col-sm-6 col-md-4")]
        public string Tag { get; set; }

        [FormlyField(DefaultValue = false, ClassName = "d-flex col-12 col-sm-6 col-md-4")]
        public bool IsRichText { get; set; }

        [FormlyArray(AddDisabled = true, RemoveDisabled = true, FieldGroupClassName = "d-flex",
            Wrappers = new[] {"card"})]
        public List<ItemForTranslationFormModel> Items { get; set; }
    }
}