using MCMS.Base.Data.Entities;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations.Item
{
    public class TranslationItemEntity : Entity
    {
        public string Value { get; set; }
        public TranslationEntity Translation { get; set; }
        public LanguageEntity Language { get; set; }
    }
}