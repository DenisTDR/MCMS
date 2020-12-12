
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Helpers;
using MCMS.Display.Link;
using Newtonsoft.Json;

namespace MCMS.Display.ModelDisplay
{
    public class TableDisplayConfig
    {
        public string Id { get; set; } = Utils.GenerateRandomHexString();
        public List<MRichLink> ItemActions { get; set; }
        public List<TableColumn> TableColumns { get; set; }
        public IEnumerable<TableColumn> TableColumnsOrdered => TableColumns.OrderBy(tc => tc.OrderIndex);
        public bool HasTableIndexColumn { get; set; }
        public bool CheckboxSelection { get; set; }
        public string ModelName { get; set; }
        public MRichLink CreateNewItemLink { get; set; }
        public string TableItemsApiUrl { get; set; }
        public bool SkipDefaultModalEventHandlers { get; set; }

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
            columns = GetColumnsOrdered().Select(tc => tc.GetDataTablesObject()),
            rowGroup = RowGroupForDataTables,
            ajax = new {url = TableItemsApiUrl},
            hasStaticIndexColumn = HasTableIndexColumn,
            checkboxSelection = CheckboxSelection,
            skipDefaultModalEventHandlers = SkipDefaultModalEventHandlers
        });

        public IEnumerable<TableColumn> GetColumnsOrdered()
        {
            var columns = TableColumns.OrderBy(tc => tc.OrderIndex).AsEnumerable();
            if (HasTableIndexColumn)
            {
                columns = columns.Prepend(new TableColumn());
            }

            if (CheckboxSelection)
            {
                columns = columns.Prepend(new TableColumn {DefaultContent = "", ClassName = "select-checkbox"});
            }

            return columns;
        }
    }
}