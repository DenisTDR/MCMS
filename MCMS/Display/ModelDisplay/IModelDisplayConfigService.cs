using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Display.ModelDisplay;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public interface IModelDisplayConfigService
    {
        Type ViewModelType { get; }
        Task<IndexPageConfig> GetIndexPageConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true);
        Task<TableDisplayConfig> GetTableConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true);
        Task<List<TableColumn>> GetTableColumns(bool excludeActionsColumn = false);
        List<DetailsField> GetDetailsFields(Type viewModelType = null);
    }
}