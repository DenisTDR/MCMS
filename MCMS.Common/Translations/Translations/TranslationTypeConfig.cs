using MCMS.Base.Data.TypeConfig;
using MCMS.Common.Translations.Translations.Item;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationTypeConfig : EntityTypeConfiguration<TranslationEntity>
    {
        public override void Configure(EntityTypeBuilder<TranslationEntity> builder)
        {
            base.Configure(builder);
            builder.HasMany(t => t.Items).WithOne(t => t.Translation).OnDelete(DeleteBehavior.Cascade);
        }
    }
}