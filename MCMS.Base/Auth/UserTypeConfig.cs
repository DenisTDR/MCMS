using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Base.Auth
{
    public class UserTypeConfig : EntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.HasMany(u => u.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        }
    }
}