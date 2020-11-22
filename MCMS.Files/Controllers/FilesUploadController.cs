using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Controllers.Api;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCMS.Files.Controllers
{
    [ApiRoute("[controller]/[action]")]
    [Authorize]
    public class FilesUploadController : ApiController
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
            Logger.LogInformation("in ckeditor Upload, file: \nname=" + upload.FileName + "\nsize=" + upload.Length +
                                  "\nname=" +
                                  upload.Name);
            var fileE = await FileUploadManager.SaveFile(upload, purpose);
            var fileViewModel = Mapper.Map<FileViewModel>(fileE);
            return Ok(new {url = fileViewModel.Url});
        }
    }
}