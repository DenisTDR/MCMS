using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;
using MCMS.Base.Helpers;

namespace MCMS.Logging.AuditLogEntries
{
    [Table("AuditLogEntries")]
    public class AuditLogEntryEntity : Entity
    {
        public MCMS.Base.Auth.User Author { get; set; }

        public string Ip { get; set; }
        public string Category { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
        public string Path { get; set; }

        [Required]
        public string TraceIdentifier { get; set; }

        public string SerializedData
        {
            get => SerializablePropertyUtils.SerializeOrEmpty(Data);
            set => Data = SerializablePropertyUtils.DeserializeOrDefault<Dictionary<string, object>>(value);
        }

        [NotMapped] public Dictionary<string, object> Data { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}