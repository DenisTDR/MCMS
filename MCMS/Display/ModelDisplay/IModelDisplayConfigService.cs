using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public interface IModelDisplayConfigService : IMConfigurable
    {
        Type ViewModelType { get; }
        bool ExcludeActionsColumn { get; set; }
        object TableItemsApiUrlValues { get; set; }
        bool UseCreateNewItemLink { get; set; }
        object CreateNewItemLinkValues { get; set; }
        bool UseModals { get; set; }
        bool ExcludeDefaultItemActions { get; set; }
        Task<IndexPageConfig> GetIndexPageConfig(IUrlHelper url);
        Task<TableDisplayConfig> GetTableConfig(IUrlHelper url);
        Task<List<TableColumn>> GetTableColumns();
        List<DetailsField> GetDetailsFields(Type viewModelType = null);
    }
}