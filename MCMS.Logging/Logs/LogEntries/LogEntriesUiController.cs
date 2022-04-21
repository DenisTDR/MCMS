using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Logging.Logs.LogEntries
{
    [Authorize(Roles = "Admin")]
    [Display(Name = "Logs")]
    public class LogEntriesUiController : GenericModalAdminUiController<LogEntryEntity, LogEntryFormModel,
        LogEntryViewModel, LogEntriesAdminApiController>
    {
        public override async Task<IndexPageConfig> GetIndexPageConfig()
        {
            var indexConfig = await base.GetIndexPageConfig();
            indexConfig.TableConfig.CreateNewItemLink = null;
            indexConfig.TableConfig.ItemActions =
                indexConfig.TableConfig.ItemActions.Where(a => a.Tag == "details").ToList();
            indexConfig.TableConfig.BatchActions.Clear();
            return indexConfig;
        }
    }
}