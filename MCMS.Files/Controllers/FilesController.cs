using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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

        public override async Task<IActionResult> Details(string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var vm = Mapper.Map<FileViewModel>(e);
            if (!vm.IsPublic)
            {
                vm.Url = vm.GetPrivateLink(Url);
            }

            return View(DetailsConfigService.Wrap(vm));
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


        public override async Task<IndexPageConfig> GetIndexPageConfig()
        {
            var config = await base.GetIndexPageConfig();

            config.TableConfig.CreateNewItemLink =
                new MRichLink("Upload new file", typeof(FilesController), nameof(Upload))
                    .WithModal()
                    .AsButton("outline-primary")
                    .WithIconClasses("fas fa-plus")
                    .WithTag("create")
                    .WithValues(null);
            return config;
        }


        [HttpGet("{id}/{fileName?}")]
        public async Task<IActionResult> DownloadFile([FromRoute] [Required] string id, [FromRoute] string fileName)
        {
            var e = await Repo.GetOneOrThrow(id);
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToAction(nameof(DownloadFile), new {id, fileName = e.OriginalName});
            }

            return ServiceProvider.GetRequiredService<FilesService>().GetFileResult(e, fileName);
        }
    }
}