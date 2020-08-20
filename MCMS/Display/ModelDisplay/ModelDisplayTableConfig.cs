using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Helpers;
using MCMS.Display.Link;
using Newtonsoft.Json;

namespace MCMS.Display.ModelDisplay
{
    public class ModelDisplayTableConfig
    {
        public string TableId { get; set; } = Utils.GenerateRandomHexString();
        public List<MRichLink> TableItemActions { get; set; }
        public List<TableColumn> TableColumns { get; set; }
        public IEnumerable<TableColumn> TableColumnsOrdered => TableColumns?.OrderBy(tc => tc.OrderIndex);
        public bool HasTableIndexColumn { get; set; }
        public string IndexPageTitle { get; set; }
        public string ModelName { get; set; }
        public MRichLink CreateNewItemLink { get; set; }
        public string TableItemsApiUrl { get; set; }

        public string TableColumnsSerializedForDataTables =>
            JsonConvert.SerializeObject(TableColumnsOrdered?.Select(tc => tc.GetDataTablesObject()));

        public object RowGroupForDataTables
        {
            get
            {
                var cols = TableColumnsOrdered?.Where(tc => tc.RowGroups).ToList();
                return cols == null || !cols.Any() ? null : new {dataSrc = cols.Select(c => c.Key)};
            }
        }

        public string ConfigObject => JsonConvert.SerializeObject(new
        {
            columns = TableColumnsOrdered?.Select(tc => tc.GetDataTablesObject()),
            rowGroup = RowGroupForDataTables
        });
    }
}