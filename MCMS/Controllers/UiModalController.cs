using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public abstract class UiModalController<T, TVm, TApiController> : UiGenericController<T, TVm, TApiController>
        where T : class, IEntity
        where TVm : class, IViewModel, IFormModel
        where TApiController : IPatchCreateApiController<TVm>
    {
        public override IActionResult Create()
        {
            ViewBag.FormParamsService = FormParamsService;
            return PartialView("ModalFormsPartials/_CreateModal");
        }
    }
}