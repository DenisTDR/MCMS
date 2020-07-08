using System.IO;
using System.Linq;
using MCMS.Files.Models;

namespace MCMS.Files
{
    public static class FileUtils
    {
        public static string GetLink(this FileEntity file)
        {
            if (string.IsNullOrEmpty(file.VirtualPath) || string.IsNullOrEmpty(file.Name) ||
                string.IsNullOrEmpty(file.Extension))
            {
                return null;
            }

            return Path.Combine("/content", file.VirtualPath, file.Name + "." + file.Extension).Replace("\\", "/");
        }

        private static readonly string[] ImageExtensions = {"jpg", "jpeg", "bmp", "gif", "png", "svg"};

        public static bool IsImageByExtension(this FileEntity file)
        {
            return ImageExtensions.Contains(file.Extension?.ToLower());
        }
    }
}