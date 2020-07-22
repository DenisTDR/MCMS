using System.ComponentModel;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Files.Controllers
{
    [DisplayName("Files")]
    public class FilesController : GenericModalAdminUiController<FileEntity, FileUploadFormModel, FileViewModel,
        FilesAdminApiController>
    {
        public override IActionResult Create()
        {
            return NotFound();
        }

        public override Task<IActionResult> Edit(string id)
        {
            FormParamsService.SchemaName = nameof(FileFormModel);
            return base.Edit(id);
        }

        [ViewLayout("_ModalLayout")]
        public IActionResult Upload()
        {
            FormParamsService.SchemaName = nameof(FileUploadFormModel);
            return View();
        }

        [ViewLayout("_ModalLayout")]
        public override async Task<IActionResult> Delete(string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            if (e.Protected)
            {
                throw new KnownException("Can't delete a protected file.");
            }

            return View($"BasicModals/DeleteModal", e);
        }


        protected override async Task<ModelDisplayTableConfig> TableConfigForIndex()
        {
            var config = await base.TableConfigForIndex();
            ViewBag.CreateNewLinkValues = null;

            config.CreateNewItemLink = new MRichLink("Upload new file", typeof(FilesController), nameof(Upload))
                .WithModal()
                .AsButton("outline-primary")
                .WithIconClasses("fas fa-plus")
                .WithValues(null);
            return config;
        }
    }
}