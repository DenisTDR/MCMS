using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using MCMS.Base.Data.Entities;

namespace MCMS.Files.Models
{
    [Table("Files")]
    public class FileEntity : Entity
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string PhysicalPath { get; set; }
        public string VirtualPath { get; set; }
        public string Purpose { get; set; }
        public bool Claimed { get; set; }
        public bool Protected { get; set; }

        public string OwnerToken { get; set; }

        [DataType(DataType.MultilineText)] public string Description { get; set; }

        [NotMapped] public string PhysicalName => Name + Extension;

        [NotMapped]
        public string PhysicalFullPath => PhysicalPath == null || PhysicalName == null
            ? null
            : Path.Combine(PhysicalPath, PhysicalName);

        public override string ToString()
        {
            return OriginalName;
        }
    }
}