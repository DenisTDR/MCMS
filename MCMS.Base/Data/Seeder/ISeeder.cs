using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MCMS.Base.Data.Seeder
{
    public interface ISeeder
    {
        Task Seed(IServiceProvider serviceProvider, JArray seedData);
        Task<JArray> BuildSeed(IServiceProvider serviceProvider);
        string SeedKey();
    }
}