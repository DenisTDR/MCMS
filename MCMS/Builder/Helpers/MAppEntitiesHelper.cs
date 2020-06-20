using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Builder.Helpers
{
    public class MAppEntitiesHelper
    {
        private MApp _app;

        public MAppEntitiesHelper(MApp app)
        {
            _app = app;
        }

        public void ProcessSpecifications(IServiceCollection services)
        {
            services.AddOptions<EntitiesConfig>().Configure(config => { config.EntityStacks.AddRange(ProcessSpecifications()); });
        }

        public List<EntityTypeStack> ProcessSpecifications()
        {
            var allTypes =
                new MSpecificationsTypeFilter().FilterMapped(_app)
                    .Where(type =>
                        !typeof(IdentityRole).IsAssignableFrom(type) && !typeof(IdentityUser).IsAssignableFrom(type))
                    .ToList();
            var entityTypes = allTypes.Where(type => typeof(IEntity).IsAssignableFrom(type)).ToList();
            var entityConfigTypes = allTypes
                .Where(type => type.IsSubclassOfGenericType(typeof(EntityTypeConfiguration<>)))
                .ToList();

            var stacks = entityTypes.Select(et => new EntityTypeStack(et)).ToList();

            foreach (var entityConfigType in entityConfigTypes)
            {
                if (!entityConfigType.TryGetGenericTypeOfImplementedGenericType(typeof(EntityTypeConfiguration<>),
                    out var genericType))
                {
                    continue;
                }

                var entityType = genericType.GenericTypeArguments.First();
                var stack = stacks.FirstOrDefault(st => st.EntityType == entityType);
                if (stack == null)
                {
                    throw new Exception(
                        $"Can't register `{entityConfigType.Name}` because it's entity (`{entityType.Name}`) isn't registered!");
                }

                stack.SetEntityTypeConfiguration(entityConfigType);
            }

            stacks.ForEach(st => st.Normalize());
            return stacks;
        }
    }
}