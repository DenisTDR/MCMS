using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationTypeConfig : EntityTypeConfiguration<TranslationEntity>
    {
        public override void Configure(EntityTypeBuilder<TranslationEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex(t => t.Slug);
        }
    }
}