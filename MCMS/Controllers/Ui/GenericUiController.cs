using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using MCMS.Data;
using MCMS.Display.ModelDisplay;
using MCMS.Helpers;
using MCMS.SwaggerFormly.FormParamsHelpers;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Ui
{
    public abstract class GenericUiController<TE, TFm, TVm, TApiController> : UiController
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : IGenericApiController<TFm, TVm>
    {
        protected virtual IModelDisplayConfigService ModelDisplayConfigService =>
            ServiceProvider.GetService(
                ModelDisplayConfigForControllerService<TE, TFm, TVm,
                        GenericUiController<TE, TFm, TVm, TApiController>, TApiController>
                    .MakeGenericTypeWithUiControllerType(GetType())) as IModelDisplayConfigService;

        protected virtual FormParamsService FormParamsService =>
            ServiceProvider.GetService<FormParamsForControllerService<TApiController, TFm>>();

        protected virtual IRepository<TE> Repo => ServiceProvider.GetService<IRepository<TE>>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.EntityName = EntityHelper.GetEntityName<TE>();
            ViewBag.FormParamsService = FormParamsService;
            ViewBag.UsesModals = UsesModals;
        }

        public override Task<IActionResult> Index()
        {
            return Task.FromResult(View("BasicPages/Index", TableConfigForIndex()) as IActionResult);
        }

        protected virtual ModelDisplayTableConfig TableConfigForIndex()
        {
            return ModelDisplayConfigService.GetTableConfig(Url, ViewBag);
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
            return View("BasicPages/Create");
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Edit([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            return View("BasicPages/Edit", e);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            return View("BasicModals/DeleteModal", e);
        }

        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed([FromRoute] string id)
        {
            await Repo.Delete(id);
            return RedirectBackOrOk();
        }
    }
}