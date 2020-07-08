using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.Entities;

namespace MCMS.Files.Models
{
    [DisplayName("File")]
    public class FileEntity : Entity
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string PhysicalPath { get; set; }
        public string VirtualPath { get; set; }
        
        public bool Claimed { get; set; }
        public bool Protected { get; set; }
        
        public string OwnerToken { get; set; }
        
        [DataType(DataType.MultilineText)] public string Description { get; set; }

        public override string ToString()
        {
            return OriginalName;
        }
    }
}