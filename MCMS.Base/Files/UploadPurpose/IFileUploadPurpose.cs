namespace MCMS.Base.Files.UploadPurpose
{
    public interface IFileUploadPurpose
    {
        public string Purpose { get;  }
        bool Private { get; set; }
        string Path { get; }
        string[] Accept { get; set; }
        public string AcceptStr { get; }
    }
}