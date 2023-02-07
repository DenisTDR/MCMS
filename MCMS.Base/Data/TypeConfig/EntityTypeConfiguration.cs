using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCMS.Base.Data.TypeConfig
{
    public class EntityTypeConfiguration<T> : IEntityTypeConfiguration, IEntityTypeConfiguration<T>
        where T : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            if (typeof(T).GetCustomAttribute<IgnoreDefaultTypeConfigurationAttribute>(true) != null)
            {
                return;
            }

            builder.ToTable(GetTableName());

            builder.HasKey(e => e.Id);

            if (typeof(ISluggable).IsAssignableFrom(typeof(T)))
            {
                builder.HasIndex(e => ((ISluggable)e).Slug).IsUnique();
            }

            if (typeof(IPublishable).IsAssignableFrom(typeof(T)))
            {
                builder.HasIndex(e => ((IPublishable)e).Published);
            }

            if (typeof(IOrderable).IsAssignableFrom(typeof(T)))
            {
                builder.HasIndex(e => ((IOrderable)e).Order);
            }

            if (typeof(ICanBeDeleted).IsAssignableFrom(typeof(T)))
            {
                builder.HasIndex(e => ((ICanBeDeleted)e).Deleted);
            }
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

        public virtual bool HasOnSaveHook { get; }

#pragma warning disable CS1998
        public virtual async Task OnSave()
#pragma warning restore CS1998
        {
        }
    }
}