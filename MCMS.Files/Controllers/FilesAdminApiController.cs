using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Helpers;
using MCMS.Base.JsonPatch;
using MCMS.Controllers.Api;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCMS.Files.Controllers
{
    public class FilesAdminApiController : GenericAdminApiController<FileEntity, FileUploadFormModel, FileViewModel>
    {
        private ILogger<FilesAdminApiController> Logger =>
            ServiceProvider.GetService<ILogger<FilesAdminApiController>>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.OrderByDescending(f => f.Created));
        }

        public override async Task<ActionResult<FileUploadFormModel>> Get(string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var fm = Mapper.Map<FileFormModel>(e);
            return Ok(fm);
        }

        public override async Task<ActionResult<FileUploadFormModel>> Patch(string id,
            JsonPatchDocument<FileUploadFormModel> doc)
        {
            if (!await Repo.Any(id))
            {
                return NotFound();
            }

            var eDoc = doc.CloneFor<FileUploadFormModel, FileEntity>();

            var e = await Repo.Patch(id, eDoc, ServiceProvider.GetService<IAdapterFactory>());
            var fm = Mapper.Map<FileFormModel>(e);

            return Ok(fm);
        }

        [ModelValidation]
        public override async Task<ActionResult<FileUploadFormModel>> Create(FileUploadFormModel fm)
        {
            var fileE = await Repo.GetOneOrThrow(fm.File.Id);
            fileE.Description = fm.Description;
            fileE.Claimed = true;
            fileE.Protected = fm.Protected;
            fileE.OwnerToken = null;
            await Repo.SaveChanges();
            return Ok(fm);
        }

        [HttpPost]
        [ModelValidation]
        public async Task<ActionResult<FileUploadFormModel>> Upload([Required] IFormFile file)
        {
            Logger.LogInformation("in Upload, file: \nname=" + file.FileName + "\nsize=" + file.Length + "\nname=" +
                                  file.Name);
            Logger.LogInformation("now delaying ...");
            await Task.Delay(new Random().Next(100, 1000));
            Logger.LogInformation("done");
            var fileE = new FileEntity()
            {
                OriginalName = file.FileName,
                OwnerToken = Utils.GenerateRandomHexString(),
                Name = Utils.GenerateRandomHexString(10),
                Extension = Path.GetExtension(file.FileName),
                VirtualPath = "virtual ceva",
                PhysicalPath = "physical ceva",
            };
            await Repo.Add(fileE);

            var fileViewModel = Mapper.Map<FileViewModel>(fileE);
            return Ok(fileViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string id, [FromQuery] string ownerToken)
        {
            if (string.IsNullOrEmpty(ownerToken))
            {
                return BadRequest();
            }

            await Repo.Delete(file => file.Id == id && file.OwnerToken == ownerToken);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<FileFormModel>> UploadMultiple(List<IFormFile> files)
        {
            Logger.LogInformation("in Upload, files: " + files.Count);
            foreach (var file in files)
            {
                Logger.LogInformation("name=" + file?.FileName + "\nsize=" + file?.Length + "\nname=" + file?.Name);
            }

            Logger.LogInformation("now delaying ...");
            await Task.Delay(new Random().Next(100, 2000));
            Logger.LogInformation("done");

            return Ok(new {ok = true});
        }
    }
}