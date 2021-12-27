using System;
using System.Linq;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;

namespace MCMS.Base.Data.TypeConfig
{
    public sealed class EntityTypeStack
    {
        public Type EntityType { get; private set; }
        public Type EntityTypeConfigurationType { get; private set; }
        public EntityTypeStack(Type entityType)
        {
            SetType(entityType);
        }

        public EntityTypeStack SetType(Type type)
        {
            if (!typeof(IEntity).IsAssignableFrom(type))
            {
                throw new Exception($"Type '{type.FullName}' does not implement '{typeof(IEntity).FullName}'.");
            }

            EntityType = type;
            return this;
        }


        public EntityTypeStack SetEntityTypeConfiguration(Type type)
        {
            if (!type.TryGetGenericTypeOfImplementedGenericType(typeof(EntityTypeConfiguration<>),
                out var genericType))
            {
                throw new Exception(
                    $"Type `{type.FullName}` does not implement `{typeof(EntityTypeConfiguration<>).FullName}`.");
            }

            if (EntityType != null)
            {
                if (EntityType != genericType.GenericTypeArguments[0])
                {
                    throw new Exception(
                        $"Can't create a entity stack with `{EntityType.Name}` and `{type.Name}` which inherits `{genericType.Name}`");
                }
            }

            EntityTypeConfigurationType = type;
            return this;
        }

        public EntityTypeStack Normalize()
        {
            if (EntityTypeConfigurationType == null)
            {
                EntityTypeConfigurationType = typeof(EntityTypeConfiguration<>).MakeGenericType(EntityType);
            }

            return this;
        }

        public IEntityTypeConfiguration GetEntityTypeConfigurationInstance()
        {
            return (IEntityTypeConfiguration) Activator.CreateInstance(EntityTypeConfigurationType);
        }

        public bool ShouldIgnoreSpecificTypeConfiguration()
        {
            return EntityTypeConfigurationType.GetCustomAttributes(false)
                .Any(attr => attr is IgnoreSpecificConfigurationAttribute);
        }

        public override string ToString()
        {
            return EntityType?.Name.Replace("Entity", "");
        }
    }
}