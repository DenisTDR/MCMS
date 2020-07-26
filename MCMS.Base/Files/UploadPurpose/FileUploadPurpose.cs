using System.Linq;

namespace MCMS.Base.Files.UploadPurpose
{
    public class FileUploadPurpose : IFileUploadPurpose
    {
        public FileUploadPurpose()
        {
        }

        public FileUploadPurpose(string purposeName)
        {
            Purpose = purposeName;
        }

        private string[] _accept;
        public bool Private { get; set; }
        public string Path { get; set; }
        public string Purpose { get; }

        public string[] Accept
        {
            get => _accept;
            set => _accept = value?.Select(ext => ext.StartsWith(".") ? ext : "." + ext)
                .Select(ext => ext.ToLower()).OrderBy(ext => ext).ToArray();
        }

        public string AcceptStr => string.Join(",", Accept ?? new string[] { });
    }
}