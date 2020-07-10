using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public interface IModelDisplayConfigService
    {
        Type ViewModelType { get; }
        ModelDisplayTableConfig GetTableConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true);
        List<TableColumn> GetTableColumns(bool excludeActionsColumn = false);
        List<DetailsField> GetDetailsFields(Type viewModelType = null);
    }
}