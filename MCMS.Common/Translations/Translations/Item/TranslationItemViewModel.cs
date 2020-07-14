using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations.Item
{
    public class TranslationItemViewModel : ViewModel
    {
        [TableColumn] public string Value { get; set; }
        public LanguageViewModel Language { get; set; }
        [TableColumn] public string LanguageName => Language?.Name ?? "-";
    }
}