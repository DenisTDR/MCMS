using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationViewModel : ViewModel
    {
        [TableColumn] public string Slug { get; set; }

        [TableColumn] public string Value { get; set; }

        public bool IsRichText { get; set; }
        public LanguageViewModel Language { get; set; }
        [TableColumn] public string LanguageName => Language?.Name ?? "-";
    }
}