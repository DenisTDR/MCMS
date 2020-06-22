using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using MCMS.Data;
using MCMS.Exceptions;
using MCMS.Helpers;
using MCMS.SwaggerFormly.FormParamsHelpers;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Ui
{
    public abstract class UiGenericController<TE, TVm, TApiController> : UiController where TE : class, IEntity
        where TVm : class, IViewModel, IFormModel
        where TApiController : IPatchCreateApiController<TVm>
    {
        protected virtual FormParamsService FormParamsService =>
            ServiceProvider.GetService<FormParamsForControllerService<TApiController, TVm>>();

        protected virtual IRepository<TE> Repo => ServiceProvider.GetService<IRepository<TE>>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.EntityName = EntityHelper.GetEntityName<TE>();
            ViewBag.FormParamsService = FormParamsService;
        }

        [HttpGet("/[controller]")]
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(View() as IActionResult);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Details([FromRoute] string id)
        {
            var e = await Repo.GetOne(id);
            if (e == null)
            {
                return NotFound();
            }

            return View(e);
        }

        [HttpGet]
        public virtual IActionResult Create()
        {
            return View("BasicPageForms/_Create");
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Edit([FromRoute] string id)
        {
            var e = await GetOneOrThrowNotFound(id);
            return View("BasicPageForms/_Edit", e);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] string id)
        {
            var e = await GetOneOrThrowNotFound(id);
            return View("BasicPageForms/_Delete", e);
        }

        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed([FromRoute] string id)
        {
            await Repo.Delete(id);
            return RedirectBackOrOk();
        }

        protected async Task<TE> GetOneOrThrowNotFound(string id)
        {
            var e = await Repo.GetOne(id);
            if (e == null)
            {
                throw new KnownException(code: 404);
            }

            return e;
        }
    }
}