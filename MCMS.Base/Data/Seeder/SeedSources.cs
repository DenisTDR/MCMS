using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCMS.Base.Data.Seeder
{
    public class SeedSources
    {
        private readonly List<string> _physical = new();
        private readonly List<(Assembly, string)> _embedded = new();

        public void Add(string physicalFile)
        {
            _physical.Add(physicalFile);
        }

        public void Add((Assembly, string) embedded)
        {
            _embedded.Add(embedded);
        }

        public List<(Assembly, string)> EmbeddedSources => _embedded.ToList();
        public List<string> PhysicalSources => _physical.ToList();
    }
}