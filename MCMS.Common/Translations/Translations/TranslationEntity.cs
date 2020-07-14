using System.Collections.Generic;
using MCMS.Base.Data.Entities;
using MCMS.Common.Translations.Translations.Item;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationEntity : Entity, ISluggable
    {
        public string Slug { get; set; }
        public bool IsRichText { get; set; }

        public List<TranslationItemEntity> Items { get; set; }

        public override string ToString()
        {
            return Slug;
        }
    }
}