using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Logging.AuditLogEntries
{
    [Authorize(Roles = "Admin")]
    [Display(Name = "Audit Log Entries")]
    public class AuditLogEntriesUiController : GenericModalAdminUiController<AuditLogEntryEntity, AuditLogEntryFormModel
        , AuditLogEntryViewModel, AuditLogEntriesAdminApiController>
    {
        public override async Task<IndexPageConfig> GetIndexPageConfig()
        {
            TableConfigService.UseCreateNewItemLink = false;

            var pageConfig = await base.GetIndexPageConfig();

            pageConfig.TableConfig.AdditionalClasses = "break-words";

            pageConfig.TableConfig.ItemActions =
                pageConfig.TableConfig.ItemActions.Where(a => a.Tag == "details").ToList();

            pageConfig.TableConfig.BatchActions = new List<BatchAction>();

            return pageConfig;
        }
    }
}