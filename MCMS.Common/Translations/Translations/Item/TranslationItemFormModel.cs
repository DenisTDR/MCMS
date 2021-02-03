using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations.Item
{
    public class TranslationItemFormModel : IFormModel
    {
        [FormlySelect(typeof(LanguagesAdminApiController), ClassName = "d-flex col-12 col-sm-3")]
        public virtual LanguageViewModel Language { get; set; }

        [FormlyField(ClassName = "d-flex-nf col-12 col-sm-9")]
        [DataType(DataType.MultilineText)]
        [FormlyFieldProp("hideExpression", "formState.parentModel.isRichText")]
        public string Value { get; set; }

        [FormlyField(ClassName = "d-flex-nf col-12 col-sm-9")]
        [FormlyCkEditor]
        [FormlyFieldProp("hideExpression", "!formState.parentModel.isRichText")]
        [FormlyFieldProp("clone-key", "value")]
        [DisplayName("Value")]
        public string ValueRich { get; set; }
    }
}