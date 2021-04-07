using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCMS.Admin
{
    public class FrameworkLibsDetails
    {
        public List<FrameworkLibDetails> Libs { get; set; }

        public void Add(FrameworkLibDetails lib)
        {
            if (Libs == null)
            {
                Libs = new List<FrameworkLibDetails>();
            }

            if (Libs.Any(l => l.Name == lib.Name))
            {
                return;
            }

            Libs.Add(lib);
        }

        public void Add(string name, string version)
        {
            Add(new FrameworkLibDetails(name, version));
        }
    }

    public class FrameworkLibDetails
    {
        public FrameworkLibDetails(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public FrameworkLibDetails(Assembly assembly)
        {
            Name = assembly.GetName().Name;
            Version = assembly.GetName().Version?.ToString() ?? "unknown";
        }

        public string Name { get; set; }
        public string Version { get; set; }
    }
}