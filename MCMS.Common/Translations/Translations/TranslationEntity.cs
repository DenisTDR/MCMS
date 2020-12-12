using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;
using MCMS.Common.Translations.Translations.Item;

namespace MCMS.Common.Translations.Translations
{
    [Table("Translations")]
    public class TranslationEntity : Entity, ISluggable
    {
        [Required] public string Slug { get; set; }
        public bool IsRichText { get; set; }
        public string Tag { get; set; }

        public List<TranslationItemEntity> Items { get; set; }

        public override string ToString()
        {
            return Slug;
        }
    }
}