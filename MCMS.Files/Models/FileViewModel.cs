using System;
using System.IO;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Files.Models
{
    public class FileViewModel : ViewModel
    {
        [TableColumn] public string OriginalName { get; set; }

        [TableColumn]
        public string Link => VirtualPath == null || Name == null
            ? "--"
            : "<a target='_blank' href='" + Path.Combine(VirtualPath, Name + Extension) + "'>" + OriginalName + "</a>";

        public string Name { get; set; }
        public string Extension { get; set; }
        public string PhysicalPath { get; set; }
        public string VirtualPath { get; set; }
        [TableColumn] public bool Claimed { get; set; }
        [TableColumn] public string Purpose { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

        [TableColumn] public string UploadTime => Created.ToString("u");
    }
}