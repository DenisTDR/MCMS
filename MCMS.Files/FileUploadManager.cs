using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using MCMS.Data;
using MCMS.Files.Models;
using MCMS.SwaggerFormly;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MCMS.Files
{
    public class FileUploadManager
    {
        private readonly ILogger<FileUploadManager> _logger;
        private readonly IRepository<FileEntity> _filesRepo;
        private readonly SwaggerConfigService _swaggerConfigService;

        public FileUploadManager(
            ILogger<FileUploadManager> logger,
            IRepository<FileEntity> filesRepo,
            SwaggerConfigService swaggerConfigService
        )
        {
            _logger = logger;
            _filesRepo = filesRepo;
            _swaggerConfigService = swaggerConfigService;
        }

        public async Task<FileEntity> SaveFile(IFormFile file, string purpose)
        {
            _swaggerConfigService.Load();

            if (!MFilesSpecifications.RegisteredPurposes.TryGetValue(purpose, out var attr))
            {
                throw new Exception("Invalid file purpose: '" + purpose + "'");
            }

            var path = attr.Path ?? "uploads";

            var fileE = new FileEntity
            {
                OriginalName = file.FileName,
                OwnerToken = Utils.GenerateRandomHexString(),
                Name = Utils.GenerateRandomHexString(32),
                Extension = Path.GetExtension(file.FileName),
                Purpose = purpose,
                VirtualPath = !attr.Private ? Path.Combine(MFiles.PublicVirtualPath, path) : "",
            };


            var physicalDir = attr.Private ? MFiles.PrivatePath : MFiles.PublicPath;
            physicalDir = Path.Combine(physicalDir, path);

            if (!Directory.Exists(physicalDir))
            {
                _logger.LogInformation("Creating directory '" + physicalDir + "'.");
                Directory.CreateDirectory(physicalDir);
            }

            var physicalPath = Path.Combine(physicalDir, fileE.PhysicalName);
            _logger.LogInformation($"Saving file '{fileE.OriginalName}' to '{physicalPath}' ...");
            await using var fileStream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            fileE.PhysicalPath = physicalDir;
            fileE = await _filesRepo.Add(fileE);
            _logger.LogInformation($"saved file '{fileE.OriginalName}' to '{physicalPath}'");

            return fileE;
        }
    }
}