using System.Collections.Generic;

namespace MCMS.Common.Translations.Seed
{
    public class TranslationSeedEntry
    {
        public string Slug { get; set; }
        public bool IsRichText { get; set; }
        public string Tag { get; set; }
        public Dictionary<string, string> Items { get; set; }
    }
}