using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
            builder.ToTable(GetTableName());

            builder.HasKey(e => e.Id);
        }
        public virtual string GetTableName()
        {
            if (typeof(T).GetCustomAttributes(true).LastOrDefault(attr => attr is TableAttribute) is
                TableAttribute toTableAttribute)
            {
                return toTableAttribute.Name;
            }

            var entityName = typeof(T).Name;
            if (entityName.EndsWith("Entity"))
            {
                entityName = entityName.Substring(0, entityName.Length - 6);
            }

            if (entityName.EndsWith("Model"))
            {
                entityName = entityName.Substring(0, entityName.Length - 5);
            }

            return entityName;
        }
    }
}