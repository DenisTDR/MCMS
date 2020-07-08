using System.ComponentModel;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
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
            FormParamsService.SetSchemaName(nameof(FileFormModel));
            return base.Edit(id);
        }

        [ViewLayout("_ModalLayout")]
        public IActionResult Upload()
        {
            FormParamsService.SetSchemaName(nameof(FileUploadFormModel));
            return View();
        }

        protected override ModelDisplayTableConfig TableConfigForIndex()
        {
            var config = base.TableConfigForIndex();
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