using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Files.UploadPurpose;
using MCMS.Base.Helpers;
using MCMS.Files.Models;
using MCMS.SwaggerFormly;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCMS.Files
{
    public class FileUploadManager
    {
        private readonly ILogger<FileUploadManager> _logger;
        private readonly IRepository<FileEntity> _filesRepo;
        private readonly SwaggerConfigService _swaggerConfigService;
        private readonly UploadPurposeOptions _options;

        public FileUploadManager(
            ILogger<FileUploadManager> logger,
            IRepository<FileEntity> filesRepo,
            SwaggerConfigService swaggerConfigService,
            IOptions<UploadPurposeOptions> options
        )
        {
            _logger = logger;
            _filesRepo = filesRepo;
            _swaggerConfigService =
                swaggerConfigService ?? throw new ArgumentNullException(nameof(swaggerConfigService));
            _options = options.Value;
        }

        public async Task<FileEntity> SaveFile(IFormFile file, string purpose)
        {
            _swaggerConfigService.Load();

            if (!_options.TryGetValue(purpose, out var attr))
            {
                throw new KnownException("Invalid file purpose: '" + purpose + "'");
            }

            if (!HasValidExtension(file.FileName, attr.Accept))
            {
                throw new KnownException(
                    $"Invalid extensions for '{file.FileName}'. Allowed extensions: {attr.AcceptStr}");
            }

            var path = attr.Path ?? "uploads";

            var fileE = new FileEntity
            {
                OriginalName = file.FileName,
                OwnerToken = Utils.GenerateRandomHexString(),
                Name = Utils.GenerateRandomHexString(32),
                Extension = Path.GetExtension(file.FileName).ToLower(),
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

        private bool HasValidExtension(string fileName, string[] allowedExtensions)
        {
            if (allowedExtensions == null || allowedExtensions.Length == 0)
            {
                return true;
            }

            fileName = fileName.ToLower();
            return allowedExtensions.Any(ext => fileName.EndsWith(ext));
        }
    }
}