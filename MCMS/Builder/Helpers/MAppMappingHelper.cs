using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.MappingConfig;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.SwaggerFormly.Models;
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
            var mappingConfigs = BuildStacks();
            // foreach (var mappingConfig in mappingConfigs)
            // {
            //     Console.WriteLine(mappingConfig.GetType().CSharpName());
            // }
            return mappingConfigs;
        }

        public List<IMappingConfig> BuildStacks()
        {
            var listMappingConfigs = new List<IMappingConfig>();

            var allTypes =
                new MSpecificationsTypeFilter().FilterMapped(_app)
                    .Where(type =>
                        !typeof(IdentityRole).IsAssignableFrom(type) && !typeof(IdentityUser).IsAssignableFrom(type))
                    .ToList();
            var entityTypes = allTypes.Where(type => typeof(IEntity).IsAssignableFrom(type)).ToList();
            var modelTypes = allTypes.Where(type =>
                typeof(IViewModel).IsAssignableFrom(type) || typeof(IFormModel).IsAssignableFrom(type)).ToList();

            var mappingConfigTypes = allTypes.Where(type => typeof(IMappingConfig).IsAssignableFrom(type)).ToList();

            var mappingStacks = entityTypes.Select(et => new EntityMappingStack(et)).ToList();

            foreach (var viewModelType in modelTypes)
            {
                var stack = mappingStacks.FirstOrDefault(st => st.CanPutModel(viewModelType));
                stack?.WithModel(viewModelType);
            }

            foreach (var mappingConfigType in mappingConfigTypes)
            {
                var bestMatchStack = mappingStacks.Select(stack => new
                {
                    stack,
                    matchPoints = stack.CanPutMappingConfig(mappingConfigType)
                }).Where(b => b.matchPoints > 0).OrderByDescending(b => b.matchPoints).FirstOrDefault();
                if (bestMatchStack != null)
                {
                    bestMatchStack.stack.PutMappingConfig(mappingConfigType);
                }
                else
                {
                    if (mappingConfigType.IsA(typeof(EntityViewModelMappingConfig<,>)) ||
                        mappingConfigType.IsA(typeof(EntityFormModelMappingConfig<,>)))
                    {
                        var stack = new EntityMappingStack();
                        stack.PutMappingConfig(mappingConfigType);
                        mappingStacks.Add(stack);
                    }
                    else
                    {
                        listMappingConfigs.Add(Activator.CreateInstance(mappingConfigType) as IMappingConfig);
                    }
                }
            }

            foreach (var entityMappingStack in mappingStacks)
            {
                entityMappingStack.TryNormalize();
            }

            listMappingConfigs.AddRange(mappingStacks.SelectMany(stack => stack.MappingConfigsInstances()));
            return listMappingConfigs;
        }
    }
}