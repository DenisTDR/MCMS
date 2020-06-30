using MCMS.Base.Data.Entities;
using MCMS.Common.Translations.Languages;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationEntity : Entity
    {
        public string Slug { get; set; }
        public string Value { get; set; }
        public bool IsRichText { get; set; }
        public LanguageEntity Language { get; set; }
    }
}