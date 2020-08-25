﻿using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using MCMS.Display.ModelDisplay;
using MCMS.SwaggerFormly.FormParamsHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Ui
{
    public abstract class GenericAdminUiController<TE, TFm, TVm, TApiController> : AdminUiController
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : IGenericApiController<TFm, TVm>
    {
        protected virtual IModelDisplayConfigForControllerService ModelDisplayConfigService =>
            ServiceProvider.GetService(
                ModelDisplayConfigForControllerService<TE, TFm, TVm,
                        GenericAdminUiController<TE, TFm, TVm, TApiController>, TApiController>
                    .MakeGenericTypeWithUiControllerType(GetType())) as IModelDisplayConfigForControllerService;

        protected virtual FormParamsService FormParamsService =>
            ServiceProvider.GetService<FormParamsForControllerService<TApiController, TFm>>();

        protected virtual IRepository<TE> Repo => ServiceProvider.GetRepo<TE>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.ModelName = TypeHelpers.GetDisplayName<TVm>();
            ViewBag.FormParamsService = FormParamsService;
            ViewBag.ModelDisplayConfigService = ModelDisplayConfigService;
            ViewBag.UsesModals = UsesModals;
        }

        public override async Task<IActionResult> Index()
        {
            return View("BasicPages/Index", await TableConfigForIndex());
        }

        public virtual Task<ModelDisplayTableConfig> TableConfigForIndex()
        {
            return ModelDisplayConfigService.GetTableConfig(Url, ViewBag);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Details([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var vm = Mapper.Map<TVm>(e);
            return View(vm);
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
        [Produces("application/json")]
        public virtual async Task<IActionResult> DeleteConfirmed([FromRoute] string id)
        {
            await Repo.Delete(id);
            return Ok();
        }
    }
}