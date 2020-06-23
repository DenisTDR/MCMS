using System.Threading.Tasks;
using MCMS.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Ui
{
    public abstract class UiModalController<T, TVm, TApiController> : UiGenericController<T, TVm, TApiController>
        where T : class, IEntity
        where TVm : class, IViewModel, IFormModel
        where TApiController : IPatchCreateApiController<TVm>
    {
        public UiModalController()
        {
            UsesModals = true;
        }
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

        [ViewLayout("_ModalLayout")]
        public override async Task<IActionResult> Delete(string id)
        {
            var e = await GetOneOrThrowNotFound(id);
            return View("ModalFormsPartials/_DeleteModal", e);
        }

        [ViewLayout("_ModalLayout")]
        public override Task<IActionResult> DeleteConfirmed(string id)
        {
            return base.DeleteConfirmed(id);
        }
    }
}