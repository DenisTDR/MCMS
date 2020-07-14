using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Common.Translations.Translations.Item
{
    public class TranslationItemTypeConfig : EntityTypeConfiguration<TranslationItemEntity>
    {
        public override void Configure(EntityTypeBuilder<TranslationItemEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(i => i.Language).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}