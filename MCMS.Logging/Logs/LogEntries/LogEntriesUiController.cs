using System.ComponentModel.DataAnnotations;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Logging.Logs.LogEntries
{
    [Authorize(Roles = "Admin")]
    [Display(Name = "LogEntries")]
    public class LogEntriesUiController : GenericModalAdminUiController<LogEntryEntity, LogEntryFormModel, LogEntryViewModel, LogEntriesAdminApiController>
    {
    }
}