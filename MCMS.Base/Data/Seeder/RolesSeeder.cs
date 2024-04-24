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
            var seedRoles = Deserialize(seedData);

            var existingRoles = await roleManager.Roles.ToListAsync();
            var rolesToAdd = seedRoles.Where(sr => existingRoles.All(er => er.Name != sr.Name));

            foreach (var roleToAdd in rolesToAdd)
            {
                logger.LogInformation("Creating auth role '{Role}'...", roleToAdd.Name);
                await roleManager.CreateAsync(new Role
                    { Name = roleToAdd.Name, Description = roleToAdd.Description, Rank = roleToAdd.Rank });
            }

            var rolesToUpdate = seedRoles
                .Select(sr => (sr, er: existingRoles.FirstOrDefault(er => er.Name == sr.Name)))
                .Where(tuple =>
                    tuple.er != null &&
                    (tuple.sr.Description != tuple.er.Description || tuple.sr.Rank != tuple.er.Rank))
                .ToList();
            foreach (var (sr, er) in rolesToUpdate)
            {
                logger.LogInformation("Updating auth role '{Role}'...", er.Name);
                er.Description = sr.Description;
                er.Rank = sr.Rank;
                await roleManager.UpdateAsync(er);
            }
        }

        public async Task<JArray> BuildSeed(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.Service<RoleManager<Role>>();
            var roles = await roleManager.Roles.Select(role => role.Name).OrderBy(rn => rn).ToListAsync();
            return JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(roles,
                Utils.DefaultJsonSerializerSettings()));
        }

        private List<RoleJsonDto> Deserialize(JArray array)
        {
            var list = new List<RoleJsonDto>();

            foreach (var jToken in array)
            {
                var dto = jToken.Type == JTokenType.String
                    ? new RoleJsonDto { Name = jToken.Value<string>() }
                    : jToken.ToObject<RoleJsonDto>();
                if (dto.Rank == 0)
                {
                    dto.Rank = 10;
                }

                list.Add(dto);
            }

            return list;
        }


        public string SeedKey() => "roles";
    }
}


public class RoleJsonDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rank { get; set; }
}