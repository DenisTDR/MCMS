using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Logging.AuditLogs.AuditLogEntries
{
    public class AuditLogEntryEntityTypeConfig : EntityTypeConfiguration<AuditLogEntryEntity>
    {
        public override void Configure(EntityTypeBuilder<AuditLogEntryEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex(p => p.Created);
            builder.HasIndex(p => p.TraceIdentifier);
        }
    }
}