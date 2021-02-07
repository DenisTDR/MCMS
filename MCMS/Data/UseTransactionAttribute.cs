using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Data
{
    public class UseTransactionAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var transaction = await context.HttpContext.RequestServices
                .GetRequiredService<BaseDbContext>().Database
                .BeginTransactionAsync();

            var executedContext = await next();

            if (executedContext.Canceled || executedContext.Exception != null && !executedContext.ExceptionHandled)
            {
                await transaction.RollbackAsync();
            }
            else
            {
                await transaction.CommitAsync();
            }
        }
    }
}