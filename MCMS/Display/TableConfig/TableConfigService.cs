using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;

namespace MCMS.Display.TableConfig
{
    public abstract class TableConfigService : ITableConfigService
    {
        public bool UseModals { get; set; }
        public object TableItemsApiUrlValues { get; set; }
        public bool UseCreateNewItemLink { get; set; } = true;
        public object CreateNewItemLinkValues { get; set; }
        public bool ExcludeDefaultItemActions { get; set; }
        public bool ExcludeActionsColumn { get; set; }
        public bool ServerSide { get; set; }
        public string TableItemsApiUrl { get; set; }
        public MRichLink CreateNewItemLink { get; set; }
        public bool UseDefaultItemAction { get; set; } = true;
        public MRichLink DefaultItemAction { get; set; }
        public abstract string ModelName { get; }

        public Func<TableConfig, TableConfig> AfterBuildHook { get; set; }

        public virtual Task<TableConfig> GetTableConfig()
        {
            var config = new TableConfig
            {
                ModelName = ModelName,
                TableColumns = GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = TableItemsApiUrl,
                ItemActions = GetItemActions(),
                BatchActions = GetBatchActions(),
                TableActions = GetTableActions(),
                ServerSide = ServerSide,
                DefaultItemAction = GetDefaultItemAction()
            };
            if (UseCreateNewItemLink)
            {
                config.CreateNewItemLink = CreateNewItemLink;
                if (UseModals)
                {
                    config.CreateNewItemLink.WithModal();
                }
            }

            if (AfterBuildHook != null) config = AfterBuildHook.Invoke(config);

            return Task.FromResult(config);
        }

        public abstract List<TableColumn> GetTableColumns();

        public virtual List<MRichLink> GetItemActions()
        {
            return new();
        }

        public virtual MRichLink GetDefaultItemAction()
        {
            return !UseDefaultItemAction
                ? null
                : DefaultItemAction ?? GetItemActions().FirstOrDefault(ia => ia.Tag == "details")?.Clone();
        }

        public virtual List<BatchAction> GetBatchActions(bool excludeDefault = false)
        {
            return new();
        }

        public virtual List<object> GetTableActions(bool excludeDefault = false)
        {
            if (excludeDefault)
            {
                return new();
            }

            return new()
            {
                "mcmsColVis",
                "pageLength"
            };
        }
    }
}