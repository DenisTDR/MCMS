using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Base.Auth
{
    public class UserRoleTypeConfig : EntityTypeConfiguration<UserRole>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            base.Configure(builder);
            builder.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId);
        }
    }
}