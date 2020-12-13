using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Helpers;
using MCMS.Base.JsonPatch;
using MCMS.Controllers.Api;
using MCMS.Files.Models;
using MCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCMS.Files.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FilesAdminApiController : CrudAdminApiController<FileEntity, FileUploadFormModel, FileViewModel>
    {
        private ILogger<FilesAdminApiController> Logger =>
            ServiceProvider.GetRequiredService<ILogger<FilesAdminApiController>>();

        private FileUploadManager FileUploadManager => ServiceProvider.GetRequiredService<FileUploadManager>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.OrderByDescending(f => f.Created));
        }

        protected override FileViewModel MapV(FileEntity e)
        {
            var vm = base.MapV(e);
            if (!vm.IsPublic)
            {
                vm.Url = vm.GetPrivateLink(Url);
            }

            return vm;
        }

        protected override List<FileViewModel> Map(List<FileEntity> entities)
        {
            return entities.Select(MapV).ToList();
        }

        public override async Task<ActionResult<FileUploadFormModel>> Get(string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var fm = Mapper.Map<FileFormModel>(e);
            return Ok(fm);
        }

        [NonAction]
        public override Task<ActionResult<ModelResponse<FileUploadFormModel>>> Patch(string id,
            JsonPatchDocument<FileUploadFormModel> doc)
        {
            throw new NotImplementedException();
        }

        [Route("{id}")]
        [HttpPatch]
        [PatchDocumentValidation]
        public async Task<ActionResult<FileFormModel>> Patch([FromRoute] [Required] string id,
            [FromBody] [Required] JsonPatchDocument<FileFormModel> doc)
        {
            if (!await Repo.Any(id))
            {
                return NotFound();
            }

            var eDoc = doc.CloneFor<FileFormModel, FileEntity>();

            var e = await Repo.Patch(id, eDoc, ServiceProvider.GetRequiredService<IAdapterFactory>());

            return Ok(GetPatchResponseModel2(e));
        }

        private ModelResponse<FileFormModel> GetPatchResponseModel2(FileEntity e)
        {
            var fm = Mapper.Map<FileFormModel>(e);
            var vm = MapV(e);
            return new DoubleModelResponse<FileFormModel, FileViewModel>(fm, vm, e.Id);
        }

        [ModelValidation]
        public override async Task<ActionResult<ModelResponse<FileUploadFormModel>>> Create(FileUploadFormModel fm)
        {
            var fileE = await Repo.GetOneOrThrow(fm.File.Id);
            fileE.Description = fm.Description;
            fileE.Claimed = true;
            fileE.Protected = fm.Protected;
            fileE.OwnerToken = null;
            await Repo.SaveChanges();

            var vm = MapV(fileE);
            return Ok(new DoubleModelResponse<FileUploadFormModel, FileViewModel>(fm, vm, fileE.Id));
        }

        [HttpPost]
        [ModelValidation]
        [RequestSizeLimit(135266304)]
        [AllowAnonymous]
        // [RequestSizeLimit((128 + 1) * 1024 * 1024)]
        public async Task<ActionResult<FileUploadFormModel>> Upload([Required] IFormFile file,
            [FromQuery] [Required] string purpose)
        {
            var requiredRoles = (Env.Get("FILE_UPLOAD_REQUIRED_ROLES") ?? "Admin").Split(",").Select(r => r.Trim());
            var matchRoles = 0;

            foreach (var requiredRole in requiredRoles)
            {
                if (User.IsInRole(requiredRole))
                {
                    matchRoles++;
                    break;
                }
            }

            if (matchRoles == 0)
            {
                return Forbid();
            }

            Logger.LogInformation("in Upload, file: \nname=" + file.FileName + "\nsize=" + file.Length + "\nname=" +
                                  file.Name);
            var fileE = await FileUploadManager.SaveFile(file, purpose);
            var fileViewModel = Mapper.Map<FileViewModel>(fileE);
            return Ok(fileViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<ActionResult<string>> Delete(string id)
        {
            throw new NotImplementedException();
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