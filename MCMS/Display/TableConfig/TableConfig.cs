using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MCMS.Display.TableConfig
{
    public class TableConfig : WithUniqueId
    {
        public int Index { get; set; }
        public string Id => UniqueId;
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
        public bool ServerSide { get; set; }
        public int ServerSideSearchDelay { get; set; } = 500;
        public int DefaultDisplayLength { get; set; } = 50;

        public string AdditionalClasses { get; set; }

        public object BuildRowGroupObject(List<TableColumn> columns)
        {
            var cols = columns?.Where(tc => tc.RowGroups).ToList();
            return cols == null || !cols.Any() ? null : new { dataSrc = cols.Select(c => c.Key) };
        }

        public string GetConfigObjectSerialized(IUrlHelper url)
        {
            var columns = GetFinalColumns();
            return JsonConvert.SerializeObject(new
            {
                columns = columns.Select(tc => tc.GetDataTablesObject(ServerSide)),
                rowGroup = BuildRowGroupObject(columns),
                ajax = new { url = TableItemsApiUrl },
                hasStaticIndexColumn = HasTableIndexColumn,
                skipDefaultModalEventHandlers = SkipDefaultModalEventHandlers,
                enableColumnSearch = EnableColumnSearch,
                checkboxSelection = CheckboxSelection,
                batchActions = BatchActions?.Select(ba => ba.GetConfigObject(url)),
                tableActions = TableActions,
                serverSide = ServerSide,
                searchDelay = ServerSideSearchDelay,
                iDisplayLength = DefaultDisplayLength
            });
        }

        public List<TableColumn> GetFinalColumns()
        {
            var columns = TableColumns.OrderBy(tc => tc.OrderIndex).AsEnumerable();
            if (HasTableIndexColumn)
            {
                columns = columns.Prepend(new TableColumn
                    { Name = "#", ClassName = "non-toggleable", Data = ServerSide ? "_index" : null });
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

            if (ItemActions.Any())
            {
                columns = columns.Append(
                    new TableColumn("<span class='col-name-hidden'>Actions</span>", "_actions", 100)
                        { Orderable = ServerClient.None, Searchable = ServerClient.None });
            }

            return columns.ToList();
        }

        protected override string GetHashSource()
        {
            return TableItemsApiUrl.Split("?").FirstOrDefaultDynamic() + "-" + Index;
        }
    }
}