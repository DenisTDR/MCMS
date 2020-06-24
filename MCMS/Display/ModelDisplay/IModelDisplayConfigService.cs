using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public interface IModelDisplayConfigService
    {
        ModelDisplayTableConfig GetTableConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true);
        List<TableColumn> GetTableColumns(bool excludeActionsColumn = false);
    }
}