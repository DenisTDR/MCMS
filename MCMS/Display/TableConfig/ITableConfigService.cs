using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;

namespace MCMS.Display.TableConfig
{
    public interface ITableConfigService
    {
        List<TableColumn> GetTableColumns();
        public bool UseModals { get; set; }
        public object TableItemsApiUrlValues { get; set; }
        public bool UseCreateNewItemLink { get; set; }
        public object CreateNewItemLinkValues { get; set; }
        public bool ExcludeDefaultItemActions { get; set; }
        public bool ServerSide { get; set; }
        Task<TableConfig> GetTableConfig();
        Func<TableConfig, TableConfig> AfterBuildHook { get; set; }
    }

    public interface ITableConfigServiceT<T> : ITableConfigService where T : IViewModel
    {
    }
}