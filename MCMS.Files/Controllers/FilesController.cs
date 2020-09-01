using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using MCMS.Base.Attributes;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;

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

            return View(vm);
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


        public override async Task<ModelDisplayTableConfig> TableConfigForIndex()
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


        [HttpGet("{id}/{fileName?}")]
        public async Task<IActionResult> DownloadFile([FromRoute] [Required] string id, [FromRoute] string fileName)
        {
            var e = await Repo.GetOneOrThrow(id);
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToAction(nameof(DownloadFile), new {id, fileName = e.OriginalName});
            }

            var filePath = e.PhysicalFullPath;
            if (!System.IO.File.Exists(filePath))
            {
                throw new KnownException("File not found on disk.", 404);
            }

            fileName = HttpUtility.UrlPathEncode(fileName).Replace(",", "%2C");
            var disposition = string.Format("inline; filename=\"{0}\"; filename*=UTF-8''{0}", fileName);
            HttpContext.Response.Headers.Add("Content-Disposition", new StringValues(disposition));

            var stream = new FileStream(filePath, FileMode.Open);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(stream, contentType);
        }
    }
}