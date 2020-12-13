using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Helpers;
using MCMS.Display.Link;
using Microsoft.AspNetCore.Mvc;
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
        public bool CheckboxSelection => BatchActions?.Any() == true;
        public string ModelName { get; set; }
        public MRichLink CreateNewItemLink { get; set; }
        public string TableItemsApiUrl { get; set; }
        public bool SkipDefaultModalEventHandlers { get; set; }
        public bool EnableColumnSearch { get; set; } = true;

        public List<BatchAction> BatchActions { get; set; }
        public List<object> TableActions { get; set; }

        public object BuildRowGroupObject(List<TableColumn> columns)
        {
            var cols = columns?.Where(tc => tc.RowGroups).ToList();
            return cols == null || !cols.Any() ? null : new {dataSrc = cols.Select(c => c.Key)};
        }

        public string GetConfigObjectSerialized(IUrlHelper url)
        {
            var columns = GetColumnsOrdered();
            return JsonConvert.SerializeObject(new
            {
                columns = columns.Select(tc => tc.GetDataTablesObject()),
                rowGroup = BuildRowGroupObject(columns),
                ajax = new {url = TableItemsApiUrl},
                hasStaticIndexColumn = HasTableIndexColumn,
                skipDefaultModalEventHandlers = SkipDefaultModalEventHandlers,
                enableColumnSearch = EnableColumnSearch,
                checkboxSelection = CheckboxSelection,
                batchActions = BatchActions?.Select(ba => ba.GetConfigObject(url)),
                tableActions = TableActions
            });
        }

        public List<TableColumn> GetColumnsOrdered()
        {
            var columns = TableColumns.OrderBy(tc => tc.OrderIndex).AsEnumerable();
            if (HasTableIndexColumn)
            {
                columns = columns.Prepend(new TableColumn {Name = "#", ClassName = "non-toggleable"});
            }

            if (CheckboxSelection)
            {
                columns = columns.Prepend(new TableColumn
                {
                    Name = "<i class=\"far fa-square\"></i>",
                    DefaultContent = "",
                    ClassName = "select-checkbox non-toggleable",
                    HeaderClassName = "select-all-checkbox"
                });
            }

            return columns.ToList();
        }
    }
}