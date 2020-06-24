using System;
using System.Collections.Generic;
using System.Reflection;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.SwaggerFormly.Models;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityMappingStack
    {
        public Type EntityType { get; private set; }
        public Type ViewModelType { get; private set; }
        public Type FormModelType { get; private set; }
        public Type ViewMappingConfigType { get; private set; }
        public Type FormMappingConfigType { get; private set; }

        public EntityMappingStack(Type type)
        {
            WithEntity(type);
        }

        public EntityMappingStack()
        {
        }

        public EntityMappingStack WithEntity(Type type)
        {
            EntityType = type;
            AssertValidTypes();
            return this;
        }

        public EntityMappingStack WithViewModel(Type type)
        {
            ViewModelType = type;
            AssertValidTypes();
            return this;
        }

        public EntityMappingStack WithFormModel(Type type)
        {
            FormModelType = type;
            AssertValidTypes();
            return this;
        }

        public EntityMappingStack WithViewMappingConfig(Type type)
        {
            ViewMappingConfigType = type;
            AssertValidTypes();
            return this;
        }

        public EntityMappingStack WithFormMappingConfig(Type type)
        {
            FormMappingConfigType = type;
            AssertValidTypes();
            return this;
        }

        public void AssertValidTypes()
        {
            TypeAssertions.AssertInheritedOrNull(EntityType, typeof(IEntity));
            TypeAssertions.AssertInheritedOrNull(ViewModelType, typeof(IViewModel));
            TypeAssertions.AssertInheritedOrNull(FormModelType, typeof(IFormModel));
            if (ViewMappingConfigType != null)
            {
                TypeAssertions.AssertInheritedGenericTypeWithTypeArguments(ViewMappingConfigType,
                    typeof(EntityViewModelMappingConfig<,>), EntityType, ViewModelType);
            }

            if (FormMappingConfigType != null)
            {
                TypeAssertions.AssertInheritedGenericTypeWithTypeArguments(FormMappingConfigType,
                    typeof(EntityFormModelMappingConfig<,>), EntityType, FormModelType);
            }
        }

        public EntityMappingStack WithModel(Type modelType)
        {
            if (!MatchModel(modelType, out var propertyInfo, out var modelName))
            {
                throw new Exception($"Can't put {modelType.FullName} in a stack.");
            }

            propertyInfo.SetValue(this, modelType);
            AssertValidTypes();

            return this;
        }

        public bool CanPutModel(Type modelType)
        {
            if (!MatchModel(modelType, out var propertyInfo, out var modelName))
            {
                return false;
            }

            if (propertyInfo?.GetValue(this) != null)
            {
                return false;
            }

            if (EntityType.Name.Replace("Entity", modelName) != modelType.Name)
            {
                return false;
            }

            return true;
        }

        private bool MatchModel(Type modelType, out PropertyInfo propertyInfo, out string modelName)
        {
            if (typeof(IViewModel).IsAssignableFrom(modelType))
            {
                propertyInfo = GetType().GetProperty(nameof(ViewModelType));
                modelName = "ViewModel";
                return true;
            }

            if (typeof(IFormModel).IsAssignableFrom(modelType))
            {
                propertyInfo = GetType().GetProperty(nameof(FormModelType));
                modelName = "FormModel";
                return true;
            }

            propertyInfo = null;
            modelName = null;
            return false;
        }

        public void PutMappingConfig(Type mappingConfigType)
        {
            if (!MatchMappingConfig(mappingConfigType, out var propertyInfo, out var existingModelType))
            {
                throw new Exception($"Can't put {mappingConfigType.FullName} in a stack.");
            }

            propertyInfo.SetValue(this, mappingConfigType);
            AssertValidTypes();
        }

        public int CanPutMappingConfig(Type mappingConfigType)
        {
            if (!MatchMappingConfig(mappingConfigType, out var propertyInfo, out var existingModelType))
            {
                return 0;
            }

            if (propertyInfo?.GetValue(this) != null)
            {
                return 0;
            }

            mappingConfigType.TryGetGenericTypeOfImplementedGenericType(typeof(EntityModelMappingConfig<,>),
                out var buildGenericType);

            return (EntityType == buildGenericType.GenericTypeArguments[0] ? 2 : 0) +
                   (existingModelType == buildGenericType.GenericTypeArguments[1] ? 1 :
                       existingModelType == null ? 0 : -3);
        }

        private bool MatchMappingConfig(Type mappingConfigType, out PropertyInfo propertyInfo,
            out Type existingModelType)
        {
            if (mappingConfigType.IsA(typeof(EntityViewModelMappingConfig<,>)))
            {
                propertyInfo = GetType().GetProperty(nameof(ViewMappingConfigType));
                existingModelType = ViewModelType;
                return true;
            }

            if (mappingConfigType.IsA(typeof(EntityFormModelMappingConfig<,>)))
            {
                propertyInfo = GetType().GetProperty(nameof(FormMappingConfigType));
                existingModelType = FormModelType;
                return true;
            }

            propertyInfo = null;
            existingModelType = null;
            return false;
        }

        public EntityMappingStack TryNormalize()
        {
            if (EntityType != null)
            {
                if (ViewMappingConfigType == null && ViewModelType != null)
                {
                    ViewMappingConfigType =
                        typeof(EntityViewModelMappingConfig<,>).MakeGenericType(EntityType, ViewModelType);
                }

                if (FormMappingConfigType == null && FormModelType != null)
                {
                    FormMappingConfigType =
                        typeof(EntityFormModelMappingConfig<,>).MakeGenericType(EntityType, FormModelType);
                }
            }

            // Console.WriteLine("###############");
            // Console.WriteLine($"{ViewMappingConfigType?.CSharpName()} of <{EntityType?.Name}, {ViewModelType?.Name}>");
            // Console.WriteLine($"{FormMappingConfigType?.CSharpName()} of <{EntityType?.Name}, {FormModelType?.Name}>");

            return this;
        }

        public List<IMappingConfig> MappingConfigsInstances()
        {
            var list = new List<IMappingConfig>();
            if (FormMappingConfigType != null)
            {
                list.Add(Activator.CreateInstance(FormMappingConfigType) as IMappingConfig);
            }

            if (ViewMappingConfigType != null)
            {
                list.Add(Activator.CreateInstance(ViewMappingConfigType) as IMappingConfig);
            }

            return list;
        }
    }
}