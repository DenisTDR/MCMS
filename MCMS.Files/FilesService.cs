using System;
using System.IO;
using System.Web;
using MCMS.Base.Exceptions;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace MCMS.Files
{
    public class FilesService
    {
        protected FilesRepository Repo;

        public FilesService(FilesRepository repo)
        {
            Repo = repo;
        }

        public FileResult GetFileResult(FileEntity file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            var fileName = file.OriginalName;
            var filePath = file.PhysicalFullPath;
            if (!File.Exists(filePath))
            {
                throw new KnownException("File not found on disk.", 404);
            }

            fileName = HttpUtility.UrlPathEncode(fileName).Replace(",", "%2C");

            var stream = new FileStream(filePath, FileMode.Open);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new FileStreamResult(stream, contentType) {FileDownloadName = fileName};
        }
    }
}