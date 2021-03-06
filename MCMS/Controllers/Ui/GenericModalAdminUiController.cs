using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Ui
{
    public abstract class
        GenericModalAdminUiController<T, TFm, TVm, TApiController> :
            GenericAdminUiController<T, TFm, TVm, TApiController>, IGenericAdminUiController
        where T : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : ICrudAdminApiController<TFm, TVm>
    {
        public GenericModalAdminUiController()
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
    }
}