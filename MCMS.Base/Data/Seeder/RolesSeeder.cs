using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Auth;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Base.Data.Seeder
{
    public class RolesSeeder : ISeeder
    {
        public async Task Seed(IServiceProvider serviceProvider, JArray seedData)
        {
            var roleManager = serviceProvider.Service<RoleManager<Role>>();
            var logger = serviceProvider.Service<ILogger<RolesSeeder>>();
            var seedRoles = seedData.ToObject<List<string>>();
            var existingRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            var rolesToAdd = seedRoles.Except(existingRoles);
            foreach (var roleToAdd in rolesToAdd)
            {
                logger.LogInformation("Creating role '{Role}'...", roleToAdd);
                await roleManager.CreateAsync(new Role { Name = roleToAdd });
            }
        }

        public async Task<JArray> BuildSeed(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.Service<RoleManager<Role>>();
            var roles = await roleManager.Roles.Select(role => role.Name).OrderBy(rn => rn).ToListAsync();
            return JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(roles,
                Utils.DefaultJsonSerializerSettings()));
        }


        public string SeedKey() => "roles";
    }
}