using System.Collections.Generic;
using System.Linq;
using MCMS.Display.Link;
using Newtonsoft.Json;

namespace MCMS.Display.ModelDisplay
{
    public class ModelDisplayTableConfig
    {
        public List<MRichLink> TableItemActions { get; set; }
        public List<TableColumn> TableColumns { get; set; }
        public IEnumerable<TableColumn> TableColumnsOrdered => TableColumns.OrderBy(tc => tc.Order);
        public bool HasTableIndexColumn { get; set; }
        public string IndexPageTitle { get; set; }
        public string ModelName { get; set; }
        public MRichLink CreateNewItemLink { get; set; }
        public string TableItemsApiUrl { get; set; }

        public string TableColumnsSerializedForDataTables =>
            JsonConvert.SerializeObject(TableColumnsOrdered.Select(tc => tc.GetDataTablesObject()));
    }
}