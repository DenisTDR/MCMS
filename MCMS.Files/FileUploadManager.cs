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

        public async Task<FileEntity> SaveFile(IFormFile file, string purpose, string newName = null)
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
            newName ??= Utils.GenerateRandomHexString(32);
            var fileE = new FileEntity
            {
                OriginalName = file.FileName,
                Size = file.Length,
                OwnerToken = Utils.GenerateRandomHexString(),
                Name = newName,
                Extension = Path.GetExtension(file.FileName).ToLower(),
                Purpose = purpose,
                VirtualPath = !attr.Private ? Path.Combine(MFiles.PublicVirtualPath, path) : "",
            };

            var physicalDir = attr.Private ? MFiles.PrivatePath : MFiles.PublicPath;
            physicalDir = Path.Combine(physicalDir, path);

            if (!Directory.Exists(physicalDir))
            {
                _logger.LogInformation("Creating directory '{Dir}'", physicalDir);
                Directory.CreateDirectory(physicalDir);
            }

            var physicalPath = Path.Combine(physicalDir, fileE.PhysicalName);
            _logger.LogInformation("Saving file '{OriginalName}' to '{PhysicalPath}' ...", fileE.OriginalName,
                physicalPath);
            await using var fileStream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();

            fileE.PhysicalPath = physicalDir;
            fileE = await _filesRepo.Add(fileE);
            _logger.LogInformation("Saved file '{OriginalName}' to '{PhysicalPath}'", fileE.OriginalName, physicalPath);

            try
            {
                EnsureFileOnDisk(physicalPath, fileE.Size);
                return fileE;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void EnsureFileOnDisk(string path, long length)
        {
            if (!File.Exists(path))
            {
                throw new KnownException("file-save-error");
            }

            if (new FileInfo(path).Length != length)
            {
                throw new KnownException("file-save-error");
            }
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