using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using MCMS.Display;
using MCMS.Display.DetailsConfig;
using MCMS.Display.ModelDisplay;
using MCMS.Display.TableConfig;
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
        where TApiController : ICrudAdminApiController<TFm, TVm>
    {
        private ITableConfigService _tableConfigService;

        public virtual ITableConfigService TableConfigService => _tableConfigService ??=
            ServiceProvider.GetRequiredService(TableConfigServiceHelper.GetTypeForUiController(GetType())) as
                ITableConfigService;

        private IDetailsConfigServiceT<TVm> _detailsConfigService;

        public virtual IDetailsConfigServiceT<TVm> DetailsConfigService => _detailsConfigService ??=
            ServiceProvider.GetRequiredService<IDetailsConfigServiceT<TVm>>();

        public virtual FormParamsService FormParamsService =>
            ServiceProvider.GetRequiredService<FormParamsForControllerService<TApiController, TFm>>();

        protected virtual IRepository<TE> Repo => ServiceProvider.GetRepo<TE>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.ModelName = TypeHelpers.GetDisplayNameOrDefault<TVm>();
            ViewBag.FormParamsService = FormParamsService;

            ViewBag.ApiControllerName = TypeHelpers.GetControllerName(typeof(TApiController));

            TableConfigService.ServerSide = true;
            TableConfigService.UseModals = UsesModals;
        }

        public override async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Headers.TryGetValue("X-Request-Modal", out var value) &&
                value.ToString().ToLower() == "true")
            {
                return View("BasicModals/IndexModal", await GetIndexPageConfig());
            }

            return View("BasicPages/Index", await GetIndexPageConfig());
        }

        [NonAction]
        public virtual async Task<IndexPageConfig> GetIndexPageConfig()
        {
            return new()
            {
                IndexPageTitle = TypeHelpers.GetDisplayName(GetType()),
                TableConfig = await TableConfigService.GetTableConfig()
            };
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Details([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var vm = Mapper.Map<TVm>(e);
            var model = new DetailsViewModelT<TVm>(vm, DetailsConfigService.GetDetailsFields());
            return View(model);
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

        [HttpGet]
        public virtual Task<IActionResult> BatchDelete([FromQuery] List<string> ids)
        {
            return Task.FromResult(View("BasicModals/BatchDeleteModal", ids) as IActionResult);
        }
    }
}