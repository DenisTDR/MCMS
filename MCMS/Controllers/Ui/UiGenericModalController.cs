using System.Threading.Tasks;
using MCMS.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Ui
{
    public abstract class UiGenericModalController<T, TFm, TApiController> : UiGenericController<T, TFm, TApiController>
        where T : class, IEntity
        where TFm : class, IFormModel
        where TApiController : IPatchCreateApiController<TFm>
    {
        public UiGenericModalController()
        {
            UsesModals = true;
        }
        [ViewLayout("_ModalLayout")]
        public override IActionResult Create()
        {
            return View("BasicModals/CreateModal");
        }

        [ViewLayout("_ModalLayout")]
        public override async Task<IActionResult> Edit([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            return View("BasicModals/EditModal", e);
        }

        [ViewLayout("_ModalLayout")]
        public override async Task<IActionResult> Delete(string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            return View("BasicModals/DeleteModal", e);
        }

        [ViewLayout("_ModalLayout")]
        public override Task<IActionResult> DeleteConfirmed(string id)
        {
            return base.DeleteConfirmed(id);
        }
    }
}