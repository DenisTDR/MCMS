using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using MCMS.Models.Dt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Logging.AuditLogs.AuditLogEntries
{
    [Authorize(Roles = "Admin")]
    public class
        AuditLogEntriesAdminApiController : CrudAdminApiController<AuditLogEntryEntity, AuditLogEntryFormModel,
        AuditLogEntryViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(c => c.Author)
                .OrderByDescending(c => c.Created)
            );
        }

        public override Task<ActionResult<DtResult<AuditLogEntryViewModel>>> DtQuery(DtParameters model)
        {
            QueryService.AlreadyOrdered = true;
            return base.DtQuery(model);
        }

        protected override Task OnCreating(AuditLogEntryEntity e)
        {
            if (e.Author != null)
                e.Author = ServiceProvider.GetRepo<MCMS.Base.Auth.User>().Attach(e.Author);

            return Task.CompletedTask;
        }
    }
}