using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;

namespace MCMS.Common.Translations.Languages
{
    [Table("Languages")]
    public class LanguageEntity : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}