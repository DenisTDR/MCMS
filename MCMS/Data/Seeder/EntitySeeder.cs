using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Data.Seeder
{
    public class EntitySeeder<T> : ISeeder where T : class, IEntity
    {
        public virtual async Task Seed(IRepository<T> repo, List<T> list, ILogger logger)
        {
            await Task.Delay(200);
            logger.LogInformation("Seeding in " + GetType().Name);
        }

        public virtual async Task<JArray> BuildSeed(IRepository<T> repo, ILogger logger)
        {
            logger.LogInformation("BuildSeed in " + GetType().Name);
            var list = await repo.GetAll();
            return JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(list,
                Utils.DefaultJsonSerializerSettings()));
        }

        public virtual async Task Seed(IServiceProvider serviceProvider, JArray seedData)
        {
            var entityData = seedData.ToObject<List<T>>();
            await Seed(serviceProvider.GetRepo<T>(), entityData,
                serviceProvider.GetRequiredService<ILogger<EntitySeeder<T>>>());
        }

        public virtual async Task<JArray> BuildSeed(IServiceProvider serviceProvider)
        {
            return await BuildSeed(serviceProvider.GetRequiredService<IRepository<T>>(),
                serviceProvider.GetRequiredService<ILogger<EntitySeeder<T>>>());
        }


        public string SeedKey() => typeof(T).Name.ToLower();
    }
}