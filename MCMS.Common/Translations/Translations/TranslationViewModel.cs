using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Common.Translations.Translations.Item;

namespace MCMS.Common.Translations.Translations
{
    [DisplayName("Translation")]
    public class TranslationViewModel : ViewModel

    {
        [TableColumn] public string Slug { get; set; }

        public bool IsRichText { get; set; }
        [DetailsField(Hidden = true)] public List<TranslationItemViewModel> Items { get; set; }

        [TableColumn(DbColumn = "Items", DbFuncFormat = "{0}.Any(ti => <condition>)<sel>ti.Value",
            Orderable = ServerClient.Client)]
        [DisplayName("Texts")]
        public string TextsPreview => string.Join(", ",
            Items?.Select(i => i.Value).Where(v => !string.IsNullOrEmpty(v)) ?? new List<string>());

        [TableColumn] public string Tag { get; set; }

        public override string ToString()
        {
            return Slug;
        }
    }
}