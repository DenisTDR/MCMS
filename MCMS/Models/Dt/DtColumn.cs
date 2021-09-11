using System.Text.Json.Serialization;
using MCMS.Base.Display.ModelDisplay;

namespace MCMS.Models.Dt
{
    public class DtColumn
    {
        public DtSearch Search { get; set; }
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Orderable { get; set; }
        public bool Searchable { get; set; }

        public DtColumn CloneForGlobalSearch(DtSearch search)
        {
            return new()
            {
                Data = Data,
                Search = search,
            };
        }
    }
}