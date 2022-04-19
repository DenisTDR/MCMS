using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Api;
using MCMS.Models.Dt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Logging.Logs.LogEntries
{
    [Authorize(Roles = "Admin")]
    public class
        LogEntriesAdminApiController : CrudAdminApiController<LogEntryEntity, LogEntryFormModel, LogEntryViewModel>
    {
        public override Task<ActionResult<DtResult<LogEntryViewModel>>> DtQuery(DtParameters model)
        {
            Repo.ChainQueryable(q => q.OrderByDescending(e => e.Created));
            QueryService.AlreadyOrdered = true;
            return base.DtQuery(model);
        }
    }
}