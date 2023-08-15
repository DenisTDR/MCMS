using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;

namespace MCMS.Display.TableConfig
{
    public interface ITableConfigService
    {
        public bool UseModals { get; set; }
        public object TableItemsApiUrlValues { get; set; }
        public bool UseCreateNewItemLink { get; set; }
        public object CreateNewItemLinkValues { get; set; }
        public bool ExcludeDefaultItemActions { get; set; }
        public bool ExcludeActionsColumn { get; set; }
        public bool ServerSide { get; set; }

        public string TableItemsApiUrl { get; set; }
        public MRichLink CreateNewItemLink { get; set; }

        Task<TableConfig> GetTableConfig();
        List<TableColumn> GetTableColumns();

        public List<MRichLink> GetItemActions();

        public List<BatchAction> GetBatchActions(bool excludeDefault = false);

        public List<object> GetTableActions(bool excludeDefault = false);

        Func<TableConfig, TableConfig> AfterBuildHook { get; set; }
    }

    public interface ITableConfigServiceT<T> : ITableConfigService where T : IViewModel
    {
    }
}