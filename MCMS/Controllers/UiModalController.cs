using System.Threading.Tasks;
using MCMS.Attributes;
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
        [ViewLayout("_ModalLayout")]
        public override IActionResult Create()
        {
            return View("ModalFormsPartials/_CreateModal");
        }

        [ViewLayout("_ModalLayout")]
        public override async Task<IActionResult> Edit([FromRoute] string id)
        {
            var e = await GetOneOrThrowNotFound(id);
            return View("ModalFormsPartials/_EditModal", e);
        }
    }
}