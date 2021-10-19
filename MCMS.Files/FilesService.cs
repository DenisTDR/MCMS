using System;
using System.IO;
using System.Web;
using MCMS.Base.Exceptions;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace MCMS.Files
{
    public class FilesService
    {
        public FileInfo GetFileInfo(FileEntity file)
        {
            var filePath = file.PhysicalFullPath;
            if (!File.Exists(filePath))
            {
                throw new KnownException("File not found on disk.", 404);
            }

            var fileInfo = new FileInfo(filePath);
            return fileInfo;
        }

        public FileResult GetFileResult(FileEntity file, string fileName = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var filePath = file.PhysicalFullPath;
            if (!File.Exists(filePath))
            {
                throw new KnownException("File not found on disk.", 404);
            }

            fileName ??= file.OriginalName;
            fileName = HttpUtility.UrlPathEncode(fileName).Replace(",", "%2C");

            var fileInfo = GetFileInfo(file);

            var stream = new FileStream(filePath, FileMode.Open);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new FileStreamResult(stream, contentType)
                { FileDownloadName = fileName, LastModified = fileInfo.LastWriteTime };
        }
    }
}