using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.MappingConfig;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Builder.Helpers
{
    public class MAppMappingHelper
    {
        private MApp _app;

        public MAppMappingHelper(MApp app)
        {
            _app = app;
        }

        public List<IMappingConfig> BuildMappingConfigs()
        {
            var stacks = BuildStacks();
            return stacks.Select(stack => (IMappingConfig) Activator.CreateInstance(stack.MappingConfigType)).ToList();
        }

        public List<EntityMappingStack> BuildStacks()
        {
            var allTypes =
                new MSpecificationsTypeFilter().FilterMapped(_app)
                    .Where(type =>
                        !typeof(IdentityRole).IsAssignableFrom(type) && !typeof(IdentityUser).IsAssignableFrom(type))
                    .ToList();
            var entityTypes = allTypes.Where(type => typeof(IEntity).IsAssignableFrom(type)).ToList();
            var viewModelTypes = allTypes.Where(type => typeof(IViewModel).IsAssignableFrom(type)).ToList();
            var mappingConfigTypes = allTypes.Where(type => typeof(IMappingConfig).IsAssignableFrom(type)).ToList();

            var mappingStacks = entityTypes.Select(et => new EntityMappingStack().WithEntityType(et)).ToList();

            foreach (var viewModelType in viewModelTypes)
            {
                var entityName = viewModelType.Name.Replace("ViewModel", "Entity");
                var stack = mappingStacks.FirstOrDefault(st =>
                    st.EntityType.Name == entityName && st.ViewModelType == null);
                if (stack != null)
                {
                    stack.WithViewModel(viewModelType);
                }
                else
                {
                    stack = new EntityMappingStack().WithViewModel(viewModelType);
                    mappingStacks.Add(stack);
                }
            }

            foreach (var mappingConfigType in mappingConfigTypes)
            {
                if (!mappingConfigType.TryGetGenericTypeOfImplementedGenericType(typeof(EntityMappingConfig<,>),
                    out var genericType))
                {
                    continue;
                }

                var entityType = genericType.GenericTypeArguments[0];
                var viewModelType = genericType.GenericTypeArguments[1];
                var stack = mappingStacks.FirstOrDefault(st =>
                    st.EntityType == entityType && st.ViewModelType == viewModelType);
                if (stack != null)
                {
                    stack.WithMappingConfig(mappingConfigType);
                }
                else
                {
                    stack = mappingStacks.FirstOrDefault(st =>
                        st.EntityType == entityType && st.ViewModelType == null);
                    if (stack != null)
                    {
                        stack.WithMappingConfig(mappingConfigType).WithViewModel(viewModelType);
                    }
                    else
                    {
                        stack = mappingStacks.FirstOrDefault(st =>
                            st.EntityType == null && st.ViewModelType == viewModelType);
                        if (stack != null)
                        {
                            stack.WithMappingConfig(mappingConfigType).WithEntityType(entityType);
                        }
                        else
                        {
                            stack = new EntityMappingStack().WithMappingConfig(mappingConfigType)
                                .WithViewModel(viewModelType).WithEntityType(entityType);
                            mappingStacks.Add(stack);
                        }
                    }
                }
            }
            
            foreach (var entityMappingStack in mappingStacks)
            {
                entityMappingStack.TryNormalize();
            }
            mappingStacks = mappingStacks.Where(stack => stack.IsValid).ToList();
            
            return mappingStacks;
        }
    }
}