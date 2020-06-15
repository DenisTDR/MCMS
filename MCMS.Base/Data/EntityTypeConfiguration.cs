using MCMS.Base.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Base.Data
{
    public class EntityTypeConfiguration<T> : IEntityTypeConfiguration, IEntityTypeConfiguration<T>
        where T : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
        }
    }
}