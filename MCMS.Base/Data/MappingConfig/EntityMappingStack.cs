using System;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityMappingStack
    {
        public Type EntityType { get; private set; }
        public Type ViewModelType { get; private set; }
        public Type MappingConfigType { get; private set; }

        public EntityMappingStack WithEntityType(Type type)
        {
            if (!typeof(IEntity).IsAssignableFrom(type))
            {
                throw new Exception($"Type '{type.FullName}' does not implement '{typeof(IEntity).FullName}'.");
            }

            EntityType = type;
            return this;
        }

        public EntityMappingStack WithViewModel(Type type)
        {
            if (!typeof(IViewModel).IsAssignableFrom(type))
            {
                throw new Exception($"Type '{type.FullName}' does not implement '{typeof(IViewModel).FullName}'.");
            }

            ViewModelType = type;
            return this;
        }

        public EntityMappingStack WithMappingConfig(Type type)
        {
            if (!type.TryGetGenericTypeOfImplementedGenericType(typeof(EntityMappingConfig<,>),
                out var genericType))
            {
                throw new Exception(
                    $"Type `{type.FullName}` does not implement `{typeof(EntityMappingConfig<,>).FullName}`.");
            }

            if (EntityType != null)
            {
                if (EntityType != genericType.GenericTypeArguments[0])
                {
                    throw new Exception(
                        $"Can't create a mapping config stack with `{EntityType.Name}` and `{type.Name}` which inherits `{genericType.Name}`");
                }
            }

            if (ViewModelType != null)
            {
                if (EntityType != genericType.GenericTypeArguments[1])
                {
                    throw new Exception(
                        $"Can't create a mapping config stack with `{ViewModelType.Name}` and `{type.Name}` which inherits `{genericType.Name}`");
                }
            }

            MappingConfigType = type;
            return this;
        }

        public EntityMappingStack TryNormalize()
        {
            if (MappingConfigType == null && ViewModelType != null && EntityType != null)
            {
                MappingConfigType = typeof(EntityMappingConfig<,>).MakeGenericType(EntityType, ViewModelType);
            }

            return this;
        }

        public bool IsValid => MappingConfigType != null;

        public IMappingConfig GetEntityTypeConfigurationInstance()
        {
            return (IMappingConfig) Activator.CreateInstance(MappingConfigType);
        }
    }
}