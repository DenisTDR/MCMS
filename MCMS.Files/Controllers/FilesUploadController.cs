using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Api;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCMS.Files.Controllers
{
    public class FilesUploadController : AdminApiController
    {
        private FileUploadManager FileUploadManager => ServiceProvider.GetRequiredService<FileUploadManager>();

        private ILogger<FilesUploadController> Logger =>
            ServiceProvider.GetRequiredService<ILogger<FilesUploadController>>();

        [HttpPost]
        [ModelValidation]
        [RequestSizeLimit(135266304)]
        // [RequestSizeLimit((128 + 1) * 1024 * 1024)]
        public async Task<ActionResult<FileUploadFormModel>> UploadCkEditor([Required] IFormFile upload,
            [FromHeader] [Required] string purpose)
        {
            Logger.LogInformation("in ckeditor Upload, file: \nname={FileName}\nsize={Length}\nname={Name}",
                upload.FileName, upload.Length, upload.Name);
            if (upload.Length == 0)
            {
                throw new KnownException("invalid-file-size-empty");
            }

            var fileE = await FileUploadManager.SaveFile(upload, purpose);
            var fileViewModel = Mapper.Map<FileViewModel>(fileE);
            return Ok(new {url = fileViewModel.Url});
        }
    }
}